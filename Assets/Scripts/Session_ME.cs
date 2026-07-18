using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class Session_ME : ISession
{
    private const int MAX_PACKET_SIZE = 16 * 1024 * 1024;

    public class Sender
    {
        public readonly List<Message> sendingMessage = new List<Message>();
        private readonly object queueLock = new object();

        public void AddMessage(Message message)
        {
            if (message == null)
            {
                return;
            }

            lock (queueLock)
            {
                sendingMessage.Add(message);
            }
        }

        public void Clear()
        {
            lock (queueLock)
            {
                sendingMessage.Clear();
            }
        }

        private Message TakeFirst()
        {
            lock (queueLock)
            {
                if (sendingMessage.Count == 0)
                {
                    return null;
                }

                Message message = sendingMessage[0];
                sendingMessage.RemoveAt(0);
                return message;
            }
        }

        public void run()
        {
            while (connected)
            {
                try
                {
                    if (!getKeyComplete)
                    {
                        Thread.Sleep(5);
                        continue;
                    }

                    Message message = TakeFirst();
                    if (message == null)
                    {
                        Thread.Sleep(5);
                        continue;
                    }

                    doSendMessage(message);
                }
                catch (Exception ex)
                {
                    Debug.LogError("Session_ME sender error: " + ex);
                    Thread.Sleep(20);
                }
            }
        }
    }

    private class MessageCollector
    {
        public void run()
        {
            bool shouldNotifyDisconnect = false;

            try
            {
                while (connected)
                {
                    Message message = readMessage();
                    if (message == null)
                    {
                        if (!connected)
                        {
                            shouldNotifyDisconnect = !isCancel;
                            break;
                        }

                        Thread.Sleep(10);
                        continue;
                    }

                    if (message.command == -27)
                    {
                        getKey(message);
                    }
                    else
                    {
                        onRecieveMsg(message);
                    }

                    Thread.Sleep(5);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Session_ME collector error: " + ex);
                shouldNotifyDisconnect = !isCancel;
            }
            finally
            {
                if (!isCancel)
                {
                    shouldNotifyDisconnect = true;
                }

                cleanNetwork();

                if (shouldNotifyDisconnect && messageHandler != null)
                {
                    if (currentTimeMillis() - timeConnected > 500)
                    {
                        messageHandler.onDisconnected(isMainSession);
                    }
                    else
                    {
                        messageHandler.onConnectionFail(isMainSession);
                    }
                }
            }
        }

        private void getKey(Message message)
        {
            try
            {
                int length = message.reader().readUnsignedByte();
                if (length <= 0)
                {
                    throw new Exception("Key length không hợp lệ: " + length);
                }

                key = new sbyte[length];
                for (int i = 0; i < length; i++)
                {
                    key[i] = message.reader().readSByte();
                }

                for (int i = 0; i < key.Length - 1; i++)
                {
                    key[i + 1] ^= key[i];
                }

                curR = 0;
                curW = 0;
                getKeyComplete = true;

                Debug.Log("getKeyComplete! Key length: " + key.Length);

                // Một số server gửi thêm thông tin session phụ sau key.
                try
                {
                    GameMidlet.IP2 = message.reader().readUTF();
                    GameMidlet.PORT2 = message.reader().readInt();
                    GameMidlet.isConnect2 = message.reader().readByte() != 0;
                }
                catch (Exception)
                {
                    GameMidlet.isConnect2 = false;
                }

                if (isMainSession && GameMidlet.isConnect2)
                {
                    GameCanvas.connect2();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Session_ME getKey error: " + ex);
                connected = false;
            }
        }

        private static bool isLargeMessage(sbyte command)
        {
            return command == -32
                || command == -66
                || command == 11
                || command == -67
                || command == -74
                || command == -87
                || command == 66
                || command == -28;
        }

        private static sbyte[] readExactly(int length)
        {
            if (length < 0 || length > MAX_PACKET_SIZE)
            {
                throw new IOException("Packet length không hợp lệ: " + length);
            }

            byte[] raw = new byte[length];
            int offset = 0;

            while (offset < length)
            {
                int read = dis.Read(raw, offset, length - offset);
                if (read <= 0)
                {
                    throw new EndOfStreamException(
                        "Socket đóng khi mới đọc " + offset + "/" + length + " byte"
                    );
                }

                offset += read;
            }

            sbyte[] data = new sbyte[length];
            if (length > 0)
            {
                Buffer.BlockCopy(raw, 0, data, 0, length);
            }
            return data;
        }

        private static int readLargeLengthByte()
        {
            sbyte value = dis.ReadSByte();
            if (getKeyComplete && key != null)
            {
                value = readKey(value);
            }

            // Giữ đúng định dạng packet 3 byte của client/server NRO này.
            return value + 128;
        }

        private Message readMessage2(sbyte command)
        {
            int low = readLargeLengthByte();
            int middle = readLargeLengthByte();
            int high = readLargeLengthByte();
            int length = (high * 256 + middle) * 256 + low;

            if (length < 0 || length > MAX_PACKET_SIZE)
            {
                throw new IOException(
                    "Large packet length không hợp lệ: command="
                    + command + ", length=" + length
                );
            }

            if (command == -28)
            {
                Debug.Log("data - length" + length);
            }

            sbyte[] data = readExactly(length);
            recvByteCount += 4 + length;
            updateTrafficText();

            if (getKeyComplete && key != null)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = readKey(data[i]);
                }
            }

            return new Message(command, data);
        }

        private Message readMessage()
        {
            try
            {
                sbyte command = dis.ReadSByte();
                if (getKeyComplete && key != null)
                {
                    command = readKey(command);
                }

                if (isLargeMessage(command))
                {
                    return readMessage2(command);
                }

                sbyte first = dis.ReadSByte();
                sbyte second = dis.ReadSByte();

                if (getKeyComplete && key != null)
                {
                    first = readKey(first);
                    second = readKey(second);
                }

                int length = ((first & 0xFF) << 8) | (second & 0xFF);
                if (length < 0 || length > MAX_PACKET_SIZE)
                {
                    throw new IOException(
                        "Packet length không hợp lệ: command="
                        + command + ", length=" + length
                    );
                }

                sbyte[] data = readExactly(length);
                recvByteCount += 3 + length;
                updateTrafficText();

                if (getKeyComplete && key != null)
                {
                    for (int i = 0; i < data.Length; i++)
                    {
                        data[i] = readKey(data[i]);
                    }
                }

                return new Message(command, data);
            }
            catch (Exception ex)
            {
                if (!isCancel)
                {
                    Debug.LogError("Session_ME readMessage error: " + ex.Message);
                }

                connected = false;
                return null;
            }
        }
    }

    protected static Session_ME instance = new Session_ME();

    private static readonly object sendLock = new object();
    private static readonly object receiveLock = new object();
    private static readonly object networkLock = new object();

    private static NetworkStream dataStream;
    private static BinaryReader dis;
    private static BinaryWriter dos;

    public static IMessageHandler messageHandler;
    public static bool isMainSession = true;

    private static TcpClient sc;
    public static volatile bool connected;
    public static volatile bool connecting;

    private static readonly Sender sender = new Sender();

    public static Thread initThread;
    public static Thread collectorThread;
    public static Thread sendThread;

    public static int sendByteCount;
    public static int recvByteCount;

    private static volatile bool getKeyComplete;
    public static sbyte[] key;

    private static sbyte curR;
    private static sbyte curW;

    private static int timeConnected;
    public static string strRecvByteCount = string.Empty;

    public static volatile bool isCancel;

    private string host;
    private int port;
    private long timeWaitConnect;

    public static int count;
    public static MyVector recieveMsg = new MyVector();

    public void clearSendingMessage()
    {
        sender.Clear();
    }

    public static Session_ME gI()
    {
        if (instance == null)
        {
            instance = new Session_ME();
        }
        return instance;
    }

    public bool isConnected()
    {
        return connected && sc != null && dis != null && dos != null;
    }

    public void setHandler(IMessageHandler msgHandler)
    {
        messageHandler = msgHandler;
    }

    public void connect(string host, int port)
    {
        if (connected || connecting || mSystem.currentTimeMillis() < timeWaitConnect)
        {
            return;
        }

        timeWaitConnect = mSystem.currentTimeMillis() + 50;
        if (isMainSession)
        {
            ServerListScreen.testConnect = -1;
        }

        close();

        this.host = host;
        this.port = port;
        isCancel = false;
        getKeyComplete = false;
        key = null;
        curR = 0;
        curW = 0;
        sendByteCount = 0;
        recvByteCount = 0;
        sender.Clear();

        lock (receiveLock)
        {
            recieveMsg.removeAllElements();
        }

        Debug.Log("host: " + host + ":" + port);

        initThread = new Thread(NetworkInit);
        initThread.IsBackground = true;
        initThread.Start();
    }

    private void NetworkInit()
    {
        connecting = true;
        Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Highest;

        try
        {
            doConnect(host, port);

            if (messageHandler != null)
            {
                messageHandler.onConnectOK(isMainSession);
            }
        }
        catch (Exception ex)
        {
            if (!isCancel)
            {
                Debug.LogError("Session_ME connect error: " + ex.Message);
            }

            cleanNetwork();

            if (!isCancel && messageHandler != null)
            {
                messageHandler.onConnectionFail(isMainSession);
            }
        }
        finally
        {
            connecting = false;
        }
    }

    public void doConnect(string host, int port)
    {
        TcpClient client = new TcpClient();
        client.NoDelay = true;
        client.Connect(host, port);

        NetworkStream stream = client.GetStream();
        BinaryReader reader = new BinaryReader(stream, new UTF8Encoding());
        BinaryWriter writer = new BinaryWriter(stream, new UTF8Encoding());

        lock (networkLock)
        {
            sc = client;
            dataStream = stream;
            dis = reader;
            dos = writer;
            connected = true;
            timeConnected = currentTimeMillis();
        }

        // Bắt tay phải gửi thẳng trước khi hàng đợi chờ getKeyComplete.
        doSendMessage(new Message(-27));

        sendThread = new Thread(sender.run);
        sendThread.IsBackground = true;
        sendThread.Start();

        collectorThread = new Thread(new MessageCollector().run);
        collectorThread.IsBackground = true;
        collectorThread.Start();
    }

    public void sendMessage(Message message)
    {
        if (message == null)
        {
            return;
        }

        count++;
        Res.outz("SEND MSG: " + message.command);
        sender.AddMessage(message);
    }

    private static void doSendMessage(Message message)
    {
        if (message == null || dos == null)
        {
            return;
        }

        sbyte[] data = message.getData();
        int length = data == null ? 0 : data.Length;
        if (length > 65535)
        {
            throw new IOException("Client packet quá lớn: " + length);
        }

        lock (sendLock)
        {
            bool encrypt = getKeyComplete && key != null && key.Length > 0;

            dos.Write(encrypt ? writeKey(message.command) : message.command);

            sbyte high = (sbyte)(length >> 8);
            sbyte low = (sbyte)(length & 0xFF);
            dos.Write(encrypt ? writeKey(high) : high);
            dos.Write(encrypt ? writeKey(low) : low);

            if (data != null)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    dos.Write(encrypt ? writeKey(data[i]) : data[i]);
                }
            }

            dos.Flush();
            sendByteCount += 3 + length;
            updateTrafficText();
        }
    }

    public static sbyte readKey(sbyte value)
    {
        if (key == null || key.Length == 0)
        {
            throw new InvalidOperationException("Session_ME read key chưa sẵn sàng");
        }

        sbyte result = (sbyte)((key[curR++] & 0xFF) ^ (value & 0xFF));
        if (curR >= key.Length)
        {
            curR = 0;
        }
        return result;
    }

    public static sbyte writeKey(sbyte value)
    {
        if (key == null || key.Length == 0)
        {
            throw new InvalidOperationException("Session_ME write key chưa sẵn sàng");
        }

        sbyte result = (sbyte)((key[curW++] & 0xFF) ^ (value & 0xFF));
        if (curW >= key.Length)
        {
            curW = 0;
        }
        return result;
    }

    private static void updateTrafficText()
    {
        int total = recvByteCount + sendByteCount;
        strRecvByteCount = total / 1024 + "." + total % 1024 / 102 + "Kb";
    }

    public static void onRecieveMsg(Message message)
    {
        if (message == null)
        {
            return;
        }

        if (Thread.CurrentThread.Name == Main.mainThreadName && messageHandler != null)
        {
            messageHandler.onMessage(message);
            return;
        }

        lock (receiveLock)
        {
            recieveMsg.addElement(message);
        }
    }

    public static void update()
    {
        while (!Controller.isStopReadMessage)
        {
            Message message = null;

            lock (receiveLock)
            {
                if (recieveMsg.size() == 0)
                {
                    break;
                }

                message = (Message)recieveMsg.elementAt(0);
                recieveMsg.removeElementAt(0);
            }

            if (message != null && messageHandler != null)
            {
                messageHandler.onMessage(message);
            }
        }
    }

    public void close()
    {
        isCancel = true;

        Thread oldInitThread = initThread;
        Thread oldCollectorThread = collectorThread;
        Thread oldSendThread = sendThread;

        cleanNetwork();

        waitForThread(oldCollectorThread);
        waitForThread(oldSendThread);
        waitForThread(oldInitThread);
    }

    private static void waitForThread(Thread thread)
    {
        if (thread == null || thread == Thread.CurrentThread)
        {
            return;
        }

        try
        {
            if (thread.IsAlive)
            {
                thread.Join(1000);
            }
        }
        catch (Exception)
        {
        }
    }

    private static void cleanNetwork()
    {
        lock (networkLock)
        {
            connected = false;
            connecting = false;
            getKeyComplete = false;
            key = null;
            curR = 0;
            curW = 0;

            try
            {
                if (sc != null)
                {
                    sc.Close();
                }
            }
            catch (Exception)
            {
            }

            try
            {
                if (dos != null)
                {
                    dos.Close();
                }
            }
            catch (Exception)
            {
            }

            try
            {
                if (dis != null)
                {
                    dis.Close();
                }
            }
            catch (Exception)
            {
            }

            try
            {
                if (dataStream != null)
                {
                    dataStream.Close();
                }
            }
            catch (Exception)
            {
            }

            sc = null;
            dataStream = null;
            dos = null;
            dis = null;
            sendThread = null;
            collectorThread = null;
            initThread = null;

            sender.Clear();

            if (isMainSession)
            {
                ServerListScreen.testConnect = 0;
            }
        }
    }

    public static int currentTimeMillis()
    {
        return Environment.TickCount;
    }

    public bool isCompareIPConnect()
    {
        return true;
    }
}