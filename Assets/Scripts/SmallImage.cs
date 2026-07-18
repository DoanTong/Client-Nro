using System;
using System.Collections.Generic;
using Assets.src.e;

public class SmallImage
{
	public static int[][] smallImg;

	public static SmallImage instance;

	public static Image[] imgbig;

	public static Small[] imgNew;

	public static MyVector vKeys = new MyVector();

	public static Image imgEmpty = null;

	public static sbyte[] newSmallVersion;

	public static int smallCount;

	public static short maxSmall;

	public static Dictionary<int, Image> imageRaw = new Dictionary<int, Image>();

	public SmallImage()
	{
		readImage();
	}

	public static void loadBigRMS()
	{
		if (imgbig == null)
		{
			imgbig = new Image[5]
			{
				GameCanvas.loadImageRMS("/img/Big0.png"),
				GameCanvas.loadImageRMS("/img/Big1.png"),
				GameCanvas.loadImageRMS("/img/Big2.png"),
				GameCanvas.loadImageRMS("/img/Big3.png"),
				GameCanvas.loadImageRMS("/img/Big4.png")
			};
		}
	}

	public static void loadBigImage()
	{
		imgEmpty = Image.createRGBImage(new int[1], 1, 1, bl: true);
	}

	public static void init()
	{
		instance = null;
		instance = new SmallImage();
	}

	public void readImage()
	{
		int num = 0;
		try
		{
			DataInputStream dataInputStream = new DataInputStream(Rms.loadRMS("NR_image"));
			short num2 = dataInputStream.readShort();
			smallImg = new int[num2][];
			for (int i = 0; i < smallImg.Length; i++)
			{
				smallImg[i] = new int[5];
			}
			for (int j = 0; j < num2; j++)
			{
				num++;
				smallImg[j][0] = dataInputStream.readUnsignedByte();
				smallImg[j][1] = dataInputStream.readShort();
				smallImg[j][2] = dataInputStream.readShort();
				smallImg[j][3] = dataInputStream.readShort();
				smallImg[j][4] = dataInputStream.readShort();
			}
		}
		catch (Exception ex)
		{
			Cout.LogError3("Loi readImage: " + ex.ToString() + "i= " + num);
		}
	}

	public static void clearHastable()
	{
	}

	public static void ensureCapacity(int id)
{
	if (id < 0)
	{
		return;
	}

	int requiredLength = id + 1;

	if (imgNew == null)
	{
		imgNew = new Small[System.Math.Max(requiredLength, 1024)];
		return;
	}

	if (requiredLength <= imgNew.Length)
	{
		return;
	}

	int newLength = System.Math.Max(
		requiredLength + 100,
		imgNew.Length * 2
	);

	Small[] newArray = new Small[newLength];
	Array.Copy(imgNew, newArray, imgNew.Length);
	imgNew = newArray;
}
	public static void createImage(int id)
	{
		if (id < 0)
		{
			return;
		}

		ensureCapacity(id);

		if (imgEmpty == null)
		{
			loadBigImage();
		}

		if (mGraphics.zoomLevel == 1)
		{
			Image localImage =
				GameCanvas.loadImage("/SmallImage/Small" + id + ".png");

			if (localImage != null)
			{
				imgNew[id] = new Small(localImage, id);
				return;
			}

			Image cachedImage = loadIconFromRMS(id);

			if (cachedImage != null)
			{
				imgNew[id] = new Small(cachedImage, id);
				return;
			}

			imgNew[id] = new Small(imgEmpty, id);
			Service.gI().requestIcon(id);
			return;
		}

		Image resourceImage =
			GameCanvas.loadImage("/SmallImage/Small" + id + ".png");

		if (resourceImage != null)
		{
			imgNew[id] = new Small(resourceImage, id);
			imageRaw[id] = resourceImage;
			return;
		}

		Image rawImage;

		if (imageRaw.TryGetValue(id, out rawImage) && rawImage != null)
		{
			imgNew[id] = new Small(rawImage, id);
			return;
		}

		Image rmsImage = loadIconFromRMS(id);

		if (rmsImage != null)
		{
			imgNew[id] = new Small(rmsImage, id);
			imageRaw[id] = rmsImage;
			return;
		}

		imgNew[id] = new Small(imgEmpty, id);
		Service.gI().requestIcon(id);
	}
	/// <summary>
	/// Load icon từ RMS (bộ nhớ cache lâu dài)
	/// </summary>
	public static Image loadIconFromRMS(int id)
	{
		try
		{
			string rmsKey = mGraphics.zoomLevel + "icon" + id;
			sbyte[] data = Rms.loadRMS(rmsKey);
			if (data != null && data.Length > 0)
			{
				Image img = Image.createImage(data, 0, data.Length);
				return img;
			}
		}
		catch (System.Exception)
		{
			// Không có trong RMS hoặc lỗi
		}
		return null;
	}

	/// <summary>
	/// Lưu icon vào RMS để cache lâu dài
	/// </summary>
	public static void saveIconToRMS(int id, sbyte[] data)
	{
		try
		{
			string rmsKey = mGraphics.zoomLevel + "icon" + id;
			Rms.saveRMS(rmsKey, data);
		}
		catch (System.Exception ex)
		{
			Cout.println("Loi saveIconToRMS: " + ex.Message);
		}
	}

	public static void drawSmallImage(
	mGraphics g,
	int id,
	int x,
	int y,
	int transform,
	int anchor)
	{
		if (g == null || id < 0)
		{
			return;
		}

		ensureCapacity(id);

		if (imgbig == null)
		{
			Small small = imgNew[id];

			if (small == null)
			{
				createImage(id);
				return;
			}

			if (small.img == null)
			{
				return;
			}

			g.drawRegion(
				small,
				0,
				0,
				mGraphics.getImageWidth(small.img),
				mGraphics.getImageHeight(small.img),
				transform,
				x,
				y,
				anchor
			);

			small.lastUsedTick = GameCanvas.gameTick;
			return;
		}

		if (smallImg != null)
		{
			bool invalidSmallData =
				id >= smallImg.Length ||
				smallImg[id] == null ||
				smallImg[id].Length < 5 ||
				smallImg[id][0] < 0 ||
				smallImg[id][0] >= imgbig.Length ||
				smallImg[id][1] >= 256 ||
				smallImg[id][2] >= 256 ||
				smallImg[id][3] >= 256 ||
				smallImg[id][4] >= 256;

			if (invalidSmallData)
			{
				Small small = imgNew[id];

				if (small == null)
				{
					createImage(id);
					return;
				}

				if (small.img != null)
				{
					small.paint(g, transform, x, y, anchor);
					small.lastUsedTick = GameCanvas.gameTick;
				}

				return;
			}

			int bigImageIndex = smallImg[id][0];

			if (imgbig[bigImageIndex] != null)
			{
				g.drawRegion(
					imgbig[bigImageIndex],
					smallImg[id][1],
					smallImg[id][2],
					smallImg[id][3],
					smallImg[id][4],
					transform,
					x,
					y,
					anchor
				);
			}

			return;
		}

		if (GameCanvas.currentScreen != GameScr.gI())
		{
			Small small = imgNew[id];

			if (small == null)
			{
				createImage(id);
				return;
			}

			if (small.img != null)
			{
				small.paint(g, transform, x, y, anchor);
				small.lastUsedTick = GameCanvas.gameTick;
			}
		}
	}
	public static void drawSmallImage(
	mGraphics g,
	int id,
	int f,
	int x,
	int y,
	int w,
	int h,
	int transform,
	int anchor)
	{
		if (g == null || id < 0 || w <= 0 || h <= 0)
		{
			return;
		}

		ensureCapacity(id);

		if (imgbig == null)
		{
			Small small = imgNew[id];

			if (small == null)
			{
				createImage(id);
				return;
			}

			if (small.img == null)
			{
				return;
			}

			g.drawRegion(
				small.img,
				0,
				f * w,
				w,
				h,
				transform,
				x,
				y,
				anchor
			);

			small.lastUsedTick = GameCanvas.gameTick;
			return;
		}

		if (smallImg != null)
		{
			bool invalidSmallData =
				id >= smallImg.Length ||
				smallImg[id] == null ||
				smallImg[id].Length < 5 ||
				smallImg[id][0] < 0 ||
				smallImg[id][0] >= imgbig.Length ||
				smallImg[id][1] >= 256 ||
				smallImg[id][2] >= 256 ||
				smallImg[id][3] >= 256 ||
				smallImg[id][4] >= 256;

			if (invalidSmallData)
			{
				Small small = imgNew[id];

				if (small == null)
				{
					createImage(id);
					return;
				}

				if (small.img != null)
				{
					small.paint(
						g,
						transform,
						f,
						x,
						y,
						w,
						h,
						anchor
					);

					small.lastUsedTick = GameCanvas.gameTick;
				}

				return;
			}

			int bigImageIndex = smallImg[id][0];

			if (bigImageIndex != 4 && imgbig[bigImageIndex] != null)
			{
				g.drawRegion(
					imgbig[bigImageIndex],
					0,
					f * w,
					w,
					h,
					transform,
					x,
					y,
					anchor
				);

				return;
			}

			Small fallbackSmall = imgNew[id];

			if (fallbackSmall == null)
			{
				createImage(id);
				return;
			}

			if (fallbackSmall.img != null)
			{
				fallbackSmall.paint(
					g,
					transform,
					f,
					x,
					y,
					w,
					h,
					anchor
				);

				fallbackSmall.lastUsedTick = GameCanvas.gameTick;
			}

			return;
		}

		if (GameCanvas.currentScreen != GameScr.gI())
		{
			Small small = imgNew[id];

			if (small == null)
			{
				createImage(id);
				return;
			}

			if (small.img != null)
			{
				small.paint(
					g,
					transform,
					f,
					x,
					y,
					w,
					h,
					anchor
				);

				small.lastUsedTick = GameCanvas.gameTick;
			}
		}
	}
	public static void update()
	{
		if (imgNew == null)
		{
			return;
		}

		if (GameCanvas.gameTick % 1000 != 0)
		{
			return;
		}

		for (int i = 0; i < imgNew.Length; i++)
		{
			Small small = imgNew[i];

			if (small == null)
			{
				continue;
			}

			small.update();
			smallCount++;
		}
	}

	/// <summary>
	/// Lưu version icon hiện tại vào RMS
	/// </summary>
	public static void saveIconVersion()
	{
		try
		{
			if (newSmallVersion != null && newSmallVersion.Length > 0)
			{
				// Tạo hash từ version array
				int versionHash = 0;
				for (int i = 0; i < newSmallVersion.Length; i++)
				{
					versionHash = versionHash * 31 + newSmallVersion[i];
				}
				string rmsKey = mGraphics.zoomLevel + "iconVersion";
				Rms.saveRMSInt(rmsKey, versionHash);
			}
		}
		catch (System.Exception ex)
		{
			Cout.println("Loi saveIconVersion: " + ex.Message);
		}
	}

	/// <summary>
	/// Kiểm tra xem version có thay đổi không (chỉ true khi ĐÃ CÓ version cũ VÀ khác version mới)
	/// </summary>
	public static bool isVersionChanged()
	{
		try
		{
			if (newSmallVersion == null || newSmallVersion.Length == 0)
			{
				return false;
			}
			
			// Load version đã lưu
			string rmsKey = mGraphics.zoomLevel + "iconVersion";
			int savedVersionHash = Rms.loadRMSInt(rmsKey, -1);
			
			// Nếu chưa có version cũ (lần đầu chạy), KHÔNG xóa cache
			if (savedVersionHash == -1)
			{
				return false;
			}
			
			// Tính hash từ version array hiện tại
			int currentVersionHash = 0;
			for (int i = 0; i < newSmallVersion.Length; i++)
			{
				currentVersionHash = currentVersionHash * 31 + newSmallVersion[i];
			}
			
			// Chỉ trả về true nếu version thực sự thay đổi
			return currentVersionHash != savedVersionHash;
		}
		catch (System.Exception)
		{
			// Nếu lỗi, KHÔNG xóa cache
			return false;
		}
	}

	/// <summary>
	/// Xóa toàn bộ cache icon (CHỈ khi user yêu cầu hoặc version thay đổi)
	/// </summary>
	public static void clearAllIconCache()
	{
		try
		{
			// Reset imgNew array
			if (imgNew != null)
			{
				for (int i = 0; i < imgNew.Length; i++)
				{
					imgNew[i] = null;
				}
			}
			// Clear imageRaw dictionary
			imageRaw.Clear();
			
			Cout.println("SmallImage: Đã xóa toàn bộ cache icon");
		}
		catch (System.Exception ex)
		{
			Cout.println("Loi clearAllIconCache: " + ex.Message);
		}
	}

	/// <summary>
	/// Kiểm tra version và xóa cache nếu version thay đổi
	/// Gọi hàm này sau khi nhận được version từ server (case -77)
	/// </summary>
	public static void checkAndClearCacheIfVersionChanged()
	{
		// Chỉ xóa cache khi version THỰC SỰ thay đổi (không xóa lần đầu)
		if (isVersionChanged())
		{
			Cout.println("SmallImage: Version thay đổi, xóa cache để tải lại icon mới");
			clearAllIconCache();
		}
		// Lưu version mới
		saveIconVersion();
	}
}
