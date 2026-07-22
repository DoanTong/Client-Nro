// namespace Assets.src.g
// {
// 	public class ClientInput : mScreen, IActionListener
// 	{
// 		public static ClientInput instance;

// 		public TField[] tf;

// 		private int x;

// 		private int y;

// 		private int w;

// 		private int h;

// 		private string[] strPaint;

// 		private int focus;

// 		private int nTf;

// 		private void init(string t)
// 		{
// 			w = GameCanvas.w - 20;
// 			if (w > 320)
// 			{
// 				w = 320;
// 			}
// 			strPaint = mFont.tahoma_7b_dark.splitFontArray(t, w - 20);
// 			x = (GameCanvas.w - w) / 2;
// 			tf = new TField[nTf];
// 			h = tf.Length * 35 + (strPaint.Length - 1) * 20 + 40;
// 			y = GameCanvas.h - h - 40 - (strPaint.Length - 1) * 20;
// 			for (int i = 0; i < tf.Length; i++)
// 			{
// 				tf[i] = new TField();
// 				tf[i].name = string.Empty;
// 				tf[i].x = x + 10;
// 				tf[i].y = y + 35 + (strPaint.Length - 1) * 20 + i * 35;
// 				tf[i].width = w - 20;
// 				tf[i].height = mScreen.ITEM_HEIGHT + 2;
// 				if (GameCanvas.isTouch)
// 				{
// 					tf[0].isFocus = false;
// 				}
// 				else
// 				{
// 					tf[0].isFocus = true;
// 				}
// 				if (!GameCanvas.isTouch)
// 				{
// 					right = tf[0].cmdClear;
// 				}
// 			}
// 			left = new Command(mResources.CLOSE, this, 1, null);
// 			center = new Command(mResources.OK, this, 2, null);
// 			if (GameCanvas.isTouch)
// 			{
// 				center.x = GameCanvas.w / 2 + 18;
// 				left.x = GameCanvas.w / 2 - 85;
// 				center.y = (left.y = y + h + 5);
// 			}
// 		}

// 		public static ClientInput gI()
// 		{
// 			if (instance == null)
// 			{
// 				instance = new ClientInput();
// 			}
// 			return instance;
// 		}

// 		public override void switchToMe()
// 		{
// 			focus = 0;
// 			base.switchToMe();
// 		}

// 		public void setInput(int type, string title)
// 		{
// 			nTf = type;
// 			init(title);
// 			switchToMe();
// 		}

// 		public override void paint(mGraphics g)
// 		{
// 			GameScr.gI().paint(g);
// 			PopUp.paintPopUp(g, x, y, w, h, -1, isButton: true);
// 			for (int i = 0; i < strPaint.Length; i++)
// 			{
// 				mFont.tahoma_7b_green2.drawString(g, strPaint[i], GameCanvas.w / 2, y + 15 + i * 20, mFont.CENTER);
// 			}
// 			for (int j = 0; j < tf.Length; j++)
// 			{
// 				tf[j].paint(g);
// 			}
// 			base.paint(g);
// 		}

// 		public override void update()
// 		{
// 			GameScr.gI().update();
// 			for (int i = 0; i < tf.Length; i++)
// 			{
// 				tf[i].update();
// 			}
// 		}

// 		public override void keyPress(int keyCode)
// 		{
// 			for (int i = 0; i < tf.Length; i++)
// 			{
// 				if (tf[i].isFocus)
// 				{
// 					tf[i].keyPressed(keyCode);
// 					break;
// 				}
// 			}
// 			base.keyPress(keyCode);
// 		}

// 		public override void updateKey()
// 		{
// 			if (GameCanvas.keyPressed[2])
// 			{
// 				focus--;
// 				if (focus < 0)
// 				{
// 					focus = tf.Length - 1;
// 				}
// 			}
// 			else if (GameCanvas.keyPressed[8])
// 			{
// 				focus++;
// 				if (focus > tf.Length - 1)
// 				{
// 					focus = 0;
// 				}
// 			}
// 			if (GameCanvas.keyPressed[2] || GameCanvas.keyPressed[8])
// 			{
// 				GameCanvas.clearKeyPressed();
// 				for (int i = 0; i < tf.Length; i++)
// 				{
// 					if (focus == i)
// 					{
// 						tf[i].isFocus = true;
// 						if (!GameCanvas.isTouch)
// 						{
// 							right = tf[i].cmdClear;
// 						}
// 					}
// 					else
// 					{
// 						tf[i].isFocus = false;
// 					}
// 					if (GameCanvas.isPointerJustRelease && GameCanvas.isPointerHoldIn(tf[i].x, tf[i].y, tf[i].width, tf[i].height))
// 					{
// 						focus = i;
// 						break;
// 					}
// 				}
// 			}
// 			base.updateKey();
// 			GameCanvas.clearKeyPressed();
// 		}

// 		public void clearScreen()
// 		{
// 			instance = null;
// 		}

// 		public void perform(int idAction, object p)
// 		{
// 			if (idAction == 1)
// 			{
// 				GameScr.instance.switchToMe();
// 				clearScreen();
// 			}
// 			if (idAction != 2)
// 			{
// 				return;
// 			}
// 			for (int i = 0; i < tf.Length; i++)
// 			{
// 				if (tf[i].getText() == null || tf[i].getText().Equals(string.Empty))
// 				{
// 					GameCanvas.startOKDlg(mResources.vuilongnhapduthongtin);
// 					return;
// 				}
// 			}
// 			Service.gI().sendClientInput(tf);
// 			GameScr.instance.switchToMe();
// 		}
// 	}
// }
using System;

namespace Assets.src.g
{
    public class ClientInput : mScreen, IActionListener
    {
        public static ClientInput instance;

        public TField[] tf;

        private int x;
        private int y;
        private int w;
        private int h;

        private string[] strPaint;

        private int focus;
        private int nTf;

        // =========================
        // GIFT CODE UI
        // =========================

        private bool isGiftCodeForm;

        private string[] giftCodes =
            new string[0];

        private int giftPage;

        private int leftPanelX;
        private int leftPanelY;
        private int leftPanelW;
        private int leftPanelH;

        private int rightPanelX;
        private int rightPanelY;
        private int rightPanelW;
        private int rightPanelH;

        private int listX;
        private int listY;
        private int listW;
        private int rowHeight;
        private int visibleRows;

        private int confirmX;
        private int confirmY;
        private int confirmW;
        private int confirmH;

        private int closeX;
        private int closeY;
        private int closeW;
        private int closeH;

        private int prevX;
        private int nextX;
        private int navY;

        private const int NAV_BUTTON_W = 28;
        private const int NAV_BUTTON_H = 22;
        private const int ROW_BUTTON_W = 56;

        // =========================
        // FORM CŨ
        // =========================

        private void init(string title)
        {
            left = null;
            center = null;
            right = null;

            w = GameCanvas.w - 20;

            if (w > 320)
            {
                w = 320;
            }

            strPaint =
                mFont.tahoma_7b_dark.splitFontArray(
                    title,
                    w - 20
                );

            x = (GameCanvas.w - w) / 2;

            tf = new TField[nTf];

            h = tf.Length * 35
                + (strPaint.Length - 1) * 20
                + 40;

            y = GameCanvas.h
                - h
                - 40
                - (strPaint.Length - 1) * 20;

            for (int i = 0; i < tf.Length; i++)
            {
                tf[i] = new TField();

                tf[i].name = string.Empty;

                tf[i].x = x + 10;

                tf[i].y = y
                    + 35
                    + (strPaint.Length - 1) * 20
                    + i * 35;

                tf[i].width = w - 20;

                tf[i].height =
                    mScreen.ITEM_HEIGHT + 2;

                if (GameCanvas.isTouch)
                {
                    tf[i].isFocus = false;
                }
                else
                {
                    tf[i].isFocus = i == 0;
                }
            }

            if (!GameCanvas.isTouch
                    && tf.Length > 0)
            {
                right = tf[0].cmdClear;
            }

            left = new Command(
                mResources.CLOSE,
                this,
                1,
                null
            );

            center = new Command(
                mResources.OK,
                this,
                2,
                null
            );

            if (GameCanvas.isTouch)
            {
                center.x =
                    GameCanvas.w / 2 + 18;

                left.x =
                    GameCanvas.w / 2 - 85;

                center.y =
                    left.y = y + h + 5;
            }
        }

        // =========================
        // FORM GIFT CODE
        // =========================

        private void initGiftCode(
            string title
        )
        {
            /*
             * Không dùng thanh Command mặc định.
             * Nút sẽ được vẽ trực tiếp trong form.
             */
            left = null;
            center = null;
            right = null;

            w = GameCanvas.w - 20;

            if (w > 620)
            {
                w = 620;
            }

            h = GameCanvas.h - 24;

            if (h > 360)
            {
                h = 360;
            }

            x = (GameCanvas.w - w) / 2;
            y = (GameCanvas.h - h) / 2;

            strPaint =
                mFont.tahoma_7b_dark.splitFontArray(
                    title,
                    w - 60
                );

            // Nút đóng X.
            closeW = 24;
            closeH = 22;

            closeX =
                x + w - closeW - 8;

            closeY = y + 7;

            int contentY = y + 38;
            int contentH = h - 48;
            int gap = 10;

            /*
             * Cột trái chiếm khoảng 42%.
             * Cột phải chứa danh sách code.
             */
            leftPanelX = x + 10;
            leftPanelY = contentY;

            leftPanelW =
                (w - 30) * 42 / 100;

            leftPanelH = contentH;

            rightPanelX =
                leftPanelX
                + leftPanelW
                + gap;

            rightPanelY = contentY;

            rightPanelW =
                x + w
                - 10
                - rightPanelX;

            rightPanelH = contentH;

            // Khởi tạo đúng 1 textfield.
            tf = new TField[nTf];

            for (int i = 0; i < tf.Length; i++)
            {
                tf[i] = new TField();

                tf[i].name = string.Empty;

                tf[i].x =
                    leftPanelX + 14;

                tf[i].y =
                    leftPanelY
                    + 62
                    + i * 35;

                tf[i].width =
                    leftPanelW - 28;

                tf[i].height =
                    mScreen.ITEM_HEIGHT + 2;

                if (GameCanvas.isTouch)
                {
                    tf[i].isFocus = false;
                }
                else
                {
                    tf[i].isFocus = i == 0;
                }
            }

            // Nút Nhập Code bên trái.
            confirmH = 27;

            confirmW = global::System.Math.Min(
				110,
				leftPanelW - 28
			);

            confirmX =
                leftPanelX
                + (leftPanelW - confirmW) / 2;

            confirmY =
                tf[0].y
                + tf[0].height
                + 16;

            // Danh sách code bên phải.
            rowHeight = 34;

            listX = rightPanelX + 8;
            listY = rightPanelY + 28;
            listW = rightPanelW - 16;

            navY =
                rightPanelY
                + rightPanelH
                - NAV_BUTTON_H
                - 6;

            visibleRows =
                (navY - listY - 4)
                / rowHeight;

            if (visibleRows < 1)
            {
                visibleRows = 1;
            }

            prevX = rightPanelX + 8;

            nextX =
                rightPanelX
                + rightPanelW
                - NAV_BUTTON_W
                - 8;

            giftPage = 0;
        }

        public static ClientInput gI()
        {
            if (instance == null)
            {
                instance = new ClientInput();
            }

            return instance;
        }

        public override void switchToMe()
        {
            focus = 0;
            base.switchToMe();
        }

        public void setInput(
            int type,
            string title
        )
        {
            isGiftCodeForm = false;

            giftCodes =
                new string[0];

            nTf = type;

            init(title);
            switchToMe();
        }

        public void setGiftCodeInput(
            int type,
            string title,
            string[] codes
        )
        {
            isGiftCodeForm = true;

            giftCodes =
                codes ?? new string[0];

            nTf = type;

            initGiftCode(title);
            switchToMe();
        }

        public override void paint(
            mGraphics g
        )
        {
            GameScr.gI().paint(g);

            if (isGiftCodeForm)
            {
                paintGiftCodeForm(g);
                return;
            }

            PopUp.paintPopUp(
                g,
                x,
                y,
                w,
                h,
                -1,
                isButton: true
            );

            for (int i = 0;
                 i < strPaint.Length;
                 i++)
            {
                mFont.tahoma_7b_green2
                    .drawString(
                        g,
                        strPaint[i],
                        GameCanvas.w / 2,
                        y + 15 + i * 20,
                        mFont.CENTER
                    );
            }

            for (int i = 0;
                 i < tf.Length;
                 i++)
            {
                tf[i].paint(g);
            }

            base.paint(g);
        }

        private void paintGiftCodeForm(
            mGraphics g
        )
        {
            // Popup lớn ngoài cùng.
            PopUp.paintPopUp(
                g,
                x,
                y,
                w,
                h,
                -1,
                isButton: true
            );

            mFont.tahoma_7b_dark.drawString(
                g,
                "Giftcode",
                x + 14,
                y + 12,
                mFont.LEFT
            );

            paintButton(
                g,
                closeX,
                closeY,
                closeW,
                closeH,
                "X",
                true
            );

            paintBox(
                g,
                leftPanelX,
                leftPanelY,
                leftPanelW,
                leftPanelH
            );

            paintBox(
                g,
                rightPanelX,
                rightPanelY,
                rightPanelW,
                rightPanelH
            );

            mFont.tahoma_7b_dark.drawString(
                g,
                "Nhập mã Giftcode",
                leftPanelX
                    + leftPanelW / 2,
                leftPanelY + 18,
                mFont.CENTER
            );

            mFont.tahoma_7b_dark.drawString(
                g,
                "Code đang hoạt động",
                rightPanelX
                    + rightPanelW / 2,
                rightPanelY + 9,
                mFont.CENTER
            );

            paintButton(
                g,
                confirmX,
                confirmY,
                confirmW,
                confirmH,
                "Nhập Code",
                false
            );

            if (giftCodes.Length == 0)
            {
                mFont.tahoma_7b_dark
                    .drawString(
                        g,
                        "Không có code phù hợp",
                        rightPanelX
                            + rightPanelW / 2,
                        listY + 18,
                        mFont.CENTER
                    );
            }
            else
            {
                int startIndex =
                    giftPage * visibleRows;

                int endIndex = global::System.Math.Min(
					startIndex + visibleRows,
					giftCodes.Length
				);

                for (int index = startIndex;
                     index < endIndex;
                     index++)
                {
                    int row =
                        index - startIndex;

                    int rowY =
                        listY
                        + row * rowHeight;

                    int buttonX =
                        listX
                        + listW
                        - ROW_BUTTON_W
                        - 5;

                    int buttonY = rowY + 4;

                    int buttonH =
                        rowHeight - 8;

                    bool selected =
                        tf[0].getText() != null
                        && tf[0].getText()
                            .Equals(
                                giftCodes[index]
                            );

                    g.setColor(
                        selected
                            ? 0xF7D38A
                            : 0xFFE3C3
                    );

                    g.fillRect(
                        listX,
                        rowY,
                        listW,
                        rowHeight - 3
                    );

                    g.setColor(0x805A18);

                    g.drawRect(
                        listX,
                        rowY,
                        listW,
                        rowHeight - 3
                    );

                    string codeText =
                        fitText(
                            giftCodes[index],
                            listW
                                - ROW_BUTTON_W
                                - 20
                        );

                    mFont.tahoma_7b_dark
                        .drawString(
                            g,
                            codeText,
                            listX + 8,
                            rowY + 10,
                            mFont.LEFT
                        );

                    paintButton(
                        g,
                        buttonX,
                        buttonY,
                        ROW_BUTTON_W,
                        buttonH,
                        "Nhập",
                        false
                    );
                }
            }

            int pageCount =
                getGiftPageCount();

            if (pageCount > 1)
            {
                paintButton(
                    g,
                    prevX,
                    navY,
                    NAV_BUTTON_W,
                    NAV_BUTTON_H,
                    "<",
                    false
                );

                paintButton(
                    g,
                    nextX,
                    navY,
                    NAV_BUTTON_W,
                    NAV_BUTTON_H,
                    ">",
                    false
                );

                mFont.tahoma_7b_dark
                    .drawString(
                        g,
                        (giftPage + 1)
                            + "/"
                            + pageCount,
                        rightPanelX
                            + rightPanelW / 2,
                        navY + 6,
                        mFont.CENTER
                    );
            }

            // Textfield được vẽ cuối.
            for (int i = 0;
                 i < tf.Length;
                 i++)
            {
                tf[i].paint(g);
            }

            // TField thay đổi clip nên phải reset.
            g.setClip(
                0,
                0,
                GameCanvas.w,
                GameCanvas.h
            );
        }

        private void paintBox(
            mGraphics g,
            int boxX,
            int boxY,
            int boxW,
            int boxH
        )
        {
            g.setColor(0xFFE2BF);

            g.fillRect(
                boxX,
                boxY,
                boxW,
                boxH
            );

            g.setColor(0x805A18);

            g.drawRect(
                boxX,
                boxY,
                boxW,
                boxH
            );

            // Vệt sáng ở mép trên.
            g.setColor(0xFFF8E7);

            g.fillRect(
                boxX + 3,
                boxY + 3,
                boxW - 6,
                4
            );
        }

        private void paintButton(
            mGraphics g,
            int buttonX,
            int buttonY,
            int buttonW,
            int buttonH,
            string text,
            bool isCloseButton
        )
        {
            g.setColor(
                isCloseButton
                    ? 0x8C250C
                    : 0x8C4A10
            );

            g.fillRect(
                buttonX,
                buttonY,
                buttonW,
                buttonH
            );

            g.setColor(
                isCloseButton
                    ? 0xEF5A27
                    : 0xED8A32
            );

            g.fillRect(
                buttonX + 2,
                buttonY + 2,
                buttonW - 4,
                buttonH - 4
            );

            g.setColor(0xFFD99A);

            g.fillRect(
                buttonX + 4,
                buttonY + 3,
                buttonW - 8,
                3
            );

            mFont.tahoma_7b_dark.drawString(
                g,
                text,
                buttonX + buttonW / 2,
                buttonY
                    + (
                        buttonH
                        - mFont.tahoma_7b_dark
                            .getHeight()
                    ) / 2,
                mFont.CENTER
            );
        }

        private string fitText(
            string value,
            int maxWidth
        )
        {
            if (value == null)
            {
                return string.Empty;
            }

            if (mFont.tahoma_7b_dark
                    .getWidth(value)
                    <= maxWidth)
            {
                return value;
            }

            string result = value;

            while (result.Length > 0
                    && mFont.tahoma_7b_dark
                        .getWidth(
                            result + "..."
                        )
                        > maxWidth)
            {
                result = result.Substring(
                    0,
                    result.Length - 1
                );
            }

            return result + "...";
        }

        private int getGiftPageCount()
        {
            if (visibleRows <= 0) {
                return 1;
            }

            int pageCount =
                (
                    giftCodes.Length
                    + visibleRows
                    - 1
                ) / visibleRows;

           return global::System.Math.Max(
				1,
				pageCount
			);
        }

        public override void update()
        {
            GameScr.gI().update();

            for (int i = 0;
                 i < tf.Length;
                 i++)
            {
                tf[i].update();
            }
        }

        public override void keyPress(
            int keyCode
        )
        {
            for (int i = 0;
                 i < tf.Length;
                 i++)
            {
                if (tf[i].isFocus)
                {
                    tf[i].keyPressed(
                        keyCode
                    );
                    break;
                }
            }

            base.keyPress(keyCode);
        }

        public override void updateKey()
        {
            if (isGiftCodeForm)
            {
                updateGiftCodeKey();
                return;
            }

            // Logic form cũ.
            if (GameCanvas.keyPressed[2])
            {
                focus--;

                if (focus < 0)
                {
                    focus =
                        tf.Length - 1;
                }
            }
            else if (GameCanvas.keyPressed[8])
            {
                focus++;

                if (focus > tf.Length - 1)
                {
                    focus = 0;
                }
            }

            if (GameCanvas.keyPressed[2]
                    || GameCanvas.keyPressed[8])
            {
                GameCanvas.clearKeyPressed();

                for (int i = 0;
                     i < tf.Length;
                     i++)
                {
                    if (focus == i)
                    {
                        tf[i].isFocus = true;

                        if (!GameCanvas.isTouch)
                        {
                            right =
                                tf[i].cmdClear;
                        }
                    }
                    else
                    {
                        tf[i].isFocus = false;
                    }
                }
            }

            base.updateKey();

            GameCanvas.clearKeyPressed();
        }

        private void updateGiftCodeKey()
        {
            bool pointerReleased =
                GameCanvas.isPointerClick
                && GameCanvas
                    .isPointerJustRelease;

            if (pointerReleased)
            {
                // Nút X.
                if (GameCanvas.isPointerHoldIn(
                        closeX,
                        closeY,
                        closeW,
                        closeH))
                {
                    GameCanvas
                        .isPointerJustRelease = false;

                    perform(1, null);
                    return;
                }

                // Nút Nhập Code.
                if (GameCanvas.isPointerHoldIn(
                        confirmX,
                        confirmY,
                        confirmW,
                        confirmH))
                {
                    GameCanvas
                        .isPointerJustRelease = false;

                    perform(2, null);
                    return;
                }

                int pageCount =
                    getGiftPageCount();

                // Trang trước.
                if (pageCount > 1
                        && GameCanvas
                            .isPointerHoldIn(
                                prevX,
                                navY,
                                NAV_BUTTON_W,
                                NAV_BUTTON_H))
                {
                    if (giftPage > 0)
                    {
                        giftPage--;
                    }

                    GameCanvas
                        .isPointerJustRelease = false;

                    return;
                }

                // Trang sau.
                if (pageCount > 1
                        && GameCanvas
                            .isPointerHoldIn(
                                nextX,
                                navY,
                                NAV_BUTTON_W,
                                NAV_BUTTON_H))
                {
                    if (giftPage
                            < pageCount - 1)
                    {
                        giftPage++;
                    }

                    GameCanvas
                        .isPointerJustRelease = false;

                    return;
                }

                int startIndex =
                    giftPage * visibleRows;

               int endIndex = global::System.Math.Min(
					startIndex + visibleRows,
					giftCodes.Length
				);

                /*
                 * Kiểm tra nút Nhập
                 * của từng dòng code.
                 */
                for (int index = startIndex;
                     index < endIndex;
                     index++)
                {
                    int row =
                        index - startIndex;

                    int rowY =
                        listY
                        + row * rowHeight;

                    int buttonX =
                        listX
                        + listW
                        - ROW_BUTTON_W
                        - 5;

                    int buttonY =
                        rowY + 4;

                    int buttonH =
                        rowHeight - 8;

                    if (GameCanvas
                            .isPointerHoldIn(
                                buttonX,
                                buttonY,
                                ROW_BUTTON_W,
                                buttonH))
                    {
                        /*
                         * Đây là đoạn tự điền code.
                         * Không dùng clipboard.
                         */
                        tf[0].setText(
                            giftCodes[index]
                        );

                        focus = 0;

                        GameCanvas
                            .isPointerJustRelease =
                            false;

                        return;
                    }
                }
            }

            int pageTotal =
                getGiftPageCount();

            // Phím trái/phải đổi trang.
            if (GameCanvas.keyPressed[4]
                    && giftPage > 0)
            {
                giftPage--;
            }
            else if (GameCanvas.keyPressed[6]
                    && giftPage
                        < pageTotal - 1)
            {
                giftPage++;
            }

            // Enter/OK xác nhận code.
            int confirmKey =
                Main.isPC ? 25 : 5;

            if (GameCanvas
                    .keyPressed[confirmKey])
            {
                GameCanvas
                    .keyPressed[confirmKey] =
                    false;

                perform(2, null);
                return;
            }

            // Nút quay lại.
            if (GameCanvas.keyPressed[12])
            {
                GameCanvas.keyPressed[12] =
                    false;

                perform(1, null);
                return;
            }

            GameCanvas.clearKeyPressed();
        }

        public void clearScreen()
        {
            instance = null;
        }

        public void perform(
            int idAction,
            object p
        )
        {
            if (idAction == 1)
            {
                GameScr.instance.switchToMe();
                clearScreen();
                return;
            }

            if (idAction != 2)
            {
                return;
            }

            for (int i = 0;
                 i < tf.Length;
                 i++)
            {
                if (tf[i].getText() == null
                        || tf[i]
                            .getText()
                            .Equals(
                                string.Empty
                            ))
                {
                    GameCanvas.startOKDlg(
                        mResources
                            .vuilongnhapduthongtin
                    );

                    return;
                }
            }

            /*
             * Giữ nguyên hệ thống cũ:
             * gửi tf[0] lên server.
             */
            Service.gI()
                .sendClientInput(tf);

            GameScr.instance.switchToMe();
        }
    }
}