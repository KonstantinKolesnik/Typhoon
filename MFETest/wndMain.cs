using System;
using System.Threading;
using MFE.Graphics.Controls;
using MFE.Graphics.Geometry;
using MFE.Graphics.Media;
using MFE.Graphics.Touching;
using MFE.LCD;
using Microsoft.SPOT;

namespace MFETest
{
    class wndMain : Window
    {
        static DateTime dt0;
        static TimeSpan ts;
        static TimeSpan sum;

        private Font font;
        private Point p;

        public wndMain()
        {
            //CrashTest();
            UIDemo();
        }

        private void CrashTest()
        {
            //Width = 800;
            //Height = 480;

            Width = 250;
            Height = 480;

            //return;
            font = Resources.GetFont(Resources.FontResources.CourierNew_10);

            for (int i = 0; i < 24; i++)
                Children.Add(new Label(10, 20 * i, font, "label" + i.ToString()) { ForeColor = Color.Brown, Tag = "Label " + i.ToString() });

            new Thread(delegate()
            {
                //int n = 0;
                while (true)
                {
                    //Debug.Print("--------- Enter loop");

                    DateTime dt = DateTime.Now;
                    int c = Children.Count;
                    for (int j = 0; j < c; j++)
                    {
                    //dt0 = DateTime.Now;



                        //Label lbl = (Label)Children.GetAt(j);
                        Label lbl = (Label)Children[j];
                        //Debug.Print("Label text set");
                        lbl.Text = dt.Ticks.ToString();
                        //Thread.Sleep(10);

                    //ts = DateTime.Now - dt0;
                    //n++;
                    //sum += ts;

                    //Debug.Print("MFE: " + ts.ToString());
                    //Debug.Print("MFE (average): " + new TimeSpan(sum.Ticks / n).ToString());

                    }
                    //Thread.Sleep(1000);

                }
            }
            ).Start();

            //Thread.Sleep(100);

            //new Thread(delegate()
            //{
            //    while (true)
            //    {
            //        Translate(1, 1);

            //        if (X >= LCDManager.ScreenWidth - 10)
            //            X = 0;
            //        if (Y >= LCDManager.ScreenHeight - 10)
            //            Y = 0;

            //        Thread.Sleep(1000);
            //    }
            //}
            //).Start();
        }
        private void UIDemo()
        {
            //Width = LCDManager.ScreenWidth;
            //Height = LCDManager.ScreenHeight;
            //Width = 320;
            //Height = 240;

            int k = Height / 240;
            font = Resources.GetFont(Resources.FontResources.CourierNew_10);

            ImageBrush brush = new ImageBrush(Resources.GetBitmap(Resources.BitmapResources.Background_800_600));
            brush.Stretch = Stretch.Fill;
            Background = brush;

            int statusbarHeight = 24;
            Panel statusbar = new Panel(0, Height - statusbarHeight, Width, statusbarHeight);
            statusbar.Background = new ImageBrush(Resources.GetBitmap(Resources.BitmapResources.Bar));
            Children.Add(statusbar);

            Label lblClock = new Label(statusbar.Width - 70, 4, font, "00:00:00");
            lblClock.ForeColor = Color.White;
            statusbar.Children.Add(lblClock);

            Level lvl2 = new Level(statusbar.Width - 120, 7, 40, 10, Orientation.Horizontal, 10);
            lvl2.Foreground = new LinearGradientBrush(Color.LimeGreen, Color.Black);
            //lvl2.Value = 50;
            statusbar.Children.Add(lvl2);

            statusbar.Children.Add(new Image(statusbar.Width - 160, 1, 23, 23, Resources.GetBitmap(Resources.BitmapResources.Drive)));
            statusbar.Children.Add(new Image(statusbar.Width - 185, 1, 23, 23, Resources.GetBitmap(Resources.BitmapResources.Mouse)));
            //statusbar.Children.Add(new Image(statusbar.Width - 210, 1, 23, 23, Resources.GetBitmap(Resources.BitmapResources.Keyboard)));

            //ToolButton btnHome = new ToolButton(10, 0, 70, statusbar.Height);
            Button btnHome = new Button(10, 0, 70, statusbar.Height, null, "", Color.Black);
            btnHome.Foreground = new ImageBrush(Resources.GetBitmap(Resources.BitmapResources.Home));
            btnHome.Border = null;
            //btnHome.Enabled = false;
            statusbar.Children.Add(btnHome);

            //-------------------------------

            //Children.Add(new Checkbox(20*k, 20*k, 20*k, 20*k));
            Children.Add(new Checkbox(20, 20, 20, 20));


            //return;
            Children.Add(new TextBlock(500, 10, 100, 100, font, "Hello world! I'm a text block. I'm very cool!")
            {
                ForeColor = Color.White,
                Background = new LinearGradientBrush(Color.Aquamarine, Color.Yellow) { Opacity = 100 },
                TextAlignment = TextAlignment.Center,
                TextVerticalAlignment = VerticalAlignment.Top,
                TextWrap = true

            });


            Level lvl = new Level(20, 40, 60, 20, Orientation.Horizontal, 10);
            lvl.Foreground = new LinearGradientBrush(Color.Blue, Color.Black);
            //lvl.Value = 0;
            Children.Add(lvl);




            ProgressBar pg = new ProgressBar(20, 80, 100, 10);
            pg.Foreground = new LinearGradientBrush(Color.LimeGreen, Color.Red);
            //pg.Foreground.Opacity = 220;
            Children.Add(pg);

            Panel pnl = new Panel(20, 100, 100, 100);
            pnl.Background = new LinearGradientBrush(Color.Blue, Color.LimeGreen);
            //pnl.Background.Opacity = 80;
            Children.Add(pnl);

            Button btn = new Button(20, 220, 80, 30, font, "<", Color.White);
            Children.Add(btn);

            Button btn2 = new Button(60, 0, 80, 25, font, "Button 2 wwwwwwww", Color.White)
                {
                    //BackgroundUnpressed = new ImageBrush(Resources.GetBitmap(Resources.BitmapResources.ButtonBackground)) { Opacity = 100 };
                    //BackgroundPressed = new ImageBrush(Resources.GetBitmap(Resources.BitmapResources.ButtonBackground)) { Opacity = 220 };
                };
            btn2.Click += delegate(object sender, EventArgs e)
            {
                wndModal dlg = new wndModal(0, 0, 0, 0);
                dlg.ShowModal();

                int a = 0;
                int b = a;

                //Close();
            };
            statusbar.Children.Add(btn2);

            RadioButtonGroup rbg = new RadioButtonGroup(20, 260, 25, 70);
            rbg.Background = new LinearGradientBrush(Color.White, Color.DarkGray);
            //rbg.Background.Opacity = 120;
            rbg.AddRadioButton(new RadioButton(5, 5, 15, true));
            rbg.AddRadioButton(new RadioButton(5, 25, 15));
            rbg.AddRadioButton(new RadioButton(5, 45, 15));
            Children.Add(rbg);


            ToolButton tbtn;

            tbtn = new ToolButton(300, 150, 128, 128);
            //tbtn.BackgroundUnpressed = new ImageBrush(Resources.GetBitmap(Resources.BitmapResources.ButtonBackground)) { Opacity = 100 };
            //tbtn.BackgroundPressed = new ImageBrush(Resources.GetBitmap(Resources.BitmapResources.ButtonBackground)) { Opacity = 220 };
            tbtn.Foreground = new ImageBrush(Resources.GetBitmap(Resources.BitmapResources.Database));
            tbtn.Foreground.Opacity = 200;
            Children.Add(tbtn);

            tbtn = new ToolButton(450, 150, 128, 128);
            tbtn.Foreground = new ImageBrush(Resources.GetBitmap(Resources.BitmapResources.Operation));
            tbtn.Foreground.Opacity = 200;
            Children.Add(tbtn);

            tbtn = new ToolButton(600, 150, 128, 128);
            tbtn.Foreground = new ImageBrush(Resources.GetBitmap(Resources.BitmapResources.Settings));
            tbtn.Foreground.Opacity = 200;
            Children.Add(tbtn);

            Children.Add(new Slider(250, 20, 150, 30, 15, Orientation.Horizontal)
            {
                Value = 80,
                Background = new ImageBrush(Resources.GetBitmap(Resources.BitmapResources.Bar)),
                Foreground = new LinearGradientBrush(Color.LightGray, Color.Black) { Opacity = 50 }
            });
            Children.Add(new Slider(200, 20, 30, 150, 12, Orientation.Vertical)
            {
                Value = 70,
                Background = new SolidColorBrush(Color.White) { Opacity = 100 }
            });

            Slider slider = new Slider(250, 60, 150, 30, 30, Orientation.Horizontal)
            {
                Value = 80,
                Background = new ImageBrush(Resources.GetBitmap(Resources.BitmapResources.Bar)),
                Foreground = new ImageBrush(Resources.GetBitmap(Resources.BitmapResources.GHILogo)) { Opacity = 200 },
            };
            Children.Add(slider);
            Label lbl = new Label(250, 100, font, slider.Value.ToString()) { ForeColor = Color.White };
            Children.Add(lbl);
            slider.ValueChanged += delegate(object sender, ValueChangedEventArgs e) { lbl.Text = e.Value.ToString(); };

            new Thread(delegate()
            {
                int v = 0;
                while (true)
                {
                    DateTime dt = DateTime.Now;

                    string hour = (dt.Hour < 10) ? "0" + dt.Hour.ToString() : dt.Hour.ToString();
                    string minute = (dt.Minute < 10) ? "0" + dt.Minute.ToString() : dt.Minute.ToString();
                    string second = (dt.Second < 10) ? "0" + dt.Second.ToString() : dt.Second.ToString();
                    string result = hour + ":" + minute + ":" + second;
                    lblClock.Text = result;

                    v += 10;
                    if (v > 100)
                        v = 0;

                    lvl.Value = v;
                    pg.Value = v;
                    lvl2.Value = v;

                    //Color temp = ((LinearGradientBrush)pnl.Background).StartColor;
                    //((LinearGradientBrush)pnl.Background).StartColor = ((LinearGradientBrush)pnl.Background).EndColor;
                    //((LinearGradientBrush)pnl.Background).EndColor = temp;
                    //pnl.Invalidate();

                    Thread.Sleep(500);
                }
            }).Start();

            //new Thread(delegate()
            //{
            //    while (true)
            //    {
            //        Translate(1, 1);

            //        if (X >= LCDManager.ScreenWidth - 100)
            //            X = 0;
            //        if (Y >= LCDManager.ScreenHeight - 100)
            //            Y = 0;

            //        Thread.Sleep(1);
            //    }
            //}
            //).Start();

            //wndModal wndModal = new wndModal();
            //wndModal.Show();
        }

        protected override void OnTouchDown(TouchEventArgs e)
        {
            base.OnTouchDown(e);

            //Background = new SolidColorBrush(Color.Aqua);
            TouchCapture.Capture(this);
            p = e.Point;
        }
        protected override void OnTouchMove(TouchEventArgs e)
        {
            if (TouchCapture.Captured == this)
            {
                Translate(e.Point.X - p.X, e.Point.Y - p.Y);
                p = e.Point;
            }
        }
        protected override void OnTouchUp(TouchEventArgs e)
        {
            base.OnTouchUp(e);

            //Background = new SolidColorBrush(Color.White);
            TouchCapture.ReleaseCapture();
        }
    }
}
