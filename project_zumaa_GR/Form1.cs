using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace project_zumaa_GR
{
    public partial class Form1 : Form
    {
		
		public class BezierCurve
		{

			public List<Point> ControlPoints;

			public float t_inc = 0.0003f;

			public Color cl = Color.Red;
			public Color clr1 = Color.White;
			public Color ftColor = Color.White;




			public BezierCurve()
			{
				ControlPoints = new List<Point>();
			}


			private float Factorial(int n)
			{
				float res = 1.0f;

				for (int i = 2; i <= n; i++)
					res *= i;

				return res;
			}

			private float C(int n, int i)
			{
				float res = Factorial(n) / (Factorial(i) * Factorial(n - i));
				return res;
			}

			private double Calc_B(float t, int i)
			{
				int n = ControlPoints.Count - 1;
				double res = C(n, i) *
								Math.Pow((1 - t), (n - i)) *
								Math.Pow(t, i);
				return res;
			}

			public PointF CalcCurvePointAtTime(float t)
			{
				PointF pt = new PointF();
				for (int i = 0; i < ControlPoints.Count; i++)
				{
					float B = (float)Calc_B(t, i);
					pt.X += B * ControlPoints[i].X;
					pt.Y += B * ControlPoints[i].Y;
				}

				return pt;
			}

			public void SetControlPoint(Point pt)
			{
				ControlPoints.Add(pt);
			}

			public Point GetPoint(int i)
			{
				return ControlPoints[i];
			}


			
			/*
			public int isCtrlPoint(int XMouse, int YMouse)
			{
				Rectangle rc;
				for (int i = 0; i < ControlPoints.Count; i++)
				{
					rc = new Rectangle(ControlPoints[i].X - 5, ControlPoints[i].Y - 5, 10, 10);
					if (XMouse >= rc.Left && XMouse <= rc.Right && YMouse >= rc.Top && YMouse <= rc.Bottom)
					{
						return i;
					}
				}
				return -1;
			}

			public void ModifyCtrlPoint(int i, int XMouse, int YMouse)
			{
				Point p = ControlPoints[i];

				p.X = XMouse;
				p.Y = YMouse;
				ControlPoints[i] = p;
			}
			*/
			


			//Draw//////////////////////////////////////////////
			private void DrawControlPoints(Graphics g)
			{
				Font Ft = new Font("System", 10);
				for (int i = 0; i < ControlPoints.Count; i++)
				{
					g.FillEllipse(new SolidBrush(clr1),
									ControlPoints[i].X - 5,
									ControlPoints[i].Y - 5, 10, 10);

					g.DrawString("P# " + i, Ft, new SolidBrush(ftColor), ControlPoints[i].X - 15, ControlPoints[i].Y - 15);
				}
			}
			private void DrawCurvePoints(Graphics g)
			{
				if (ControlPoints.Count <= 0)
					return;

				PointF curvePoint;
				for (float t = 0.0f; t <= 1.0; t += t_inc)
				{
					curvePoint = CalcCurvePointAtTime(t);
					g.FillEllipse(new SolidBrush(cl),
									curvePoint.X - 4, curvePoint.Y - 4,
									8, 8);
				}
			}
			public void DrawCurve(Graphics g)
			{
				//DrawControlPoints(g);
				DrawCurvePoints(g);
			}
		}
		class Transform
		{
			public int x, y;
			public PointF ps;
			public PointF pe;
			public Bitmap image;


			public void DrawYourSelf(Graphics g)
			{
				Pen p = new Pen(Color.Green, 5);
				g.DrawLine(p, ps.X, ps.Y, pe.X, pe.Y);
				g.FillEllipse(Brushes.Blue, ps.X - 5, ps.Y - 5, 10, 10);
				g.FillEllipse(Brushes.Red, pe.X - 5, pe.Y - 5, 10, 10);

			}

			public void Translation(float tx, float ty)
			{
				ps.X += tx;
				ps.Y += ty;

				pe.X += tx;
				pe.Y += ty;
			}

			public void Rotate(float th_degg)
			{
				double xn, yn;
				double th_red = th_degg * Math.PI / 180;

				xn = ps.X * Math.Cos(th_red) - ps.Y * Math.Sin(th_red);
				yn = ps.X * Math.Sin(th_red) + ps.Y * Math.Cos(th_red);
				ps.X = (float)xn;
				ps.Y = (float)yn;

				xn = pe.X * Math.Cos(th_red) - pe.Y * Math.Sin(th_red);
				yn = pe.X * Math.Sin(th_red) + pe.Y * Math.Cos(th_red);
				pe.X = (float)xn;
				pe.Y = (float)yn;

			}

			public void RotateAround(float th_degg, PointF refPt)
			{
				Translation(-refPt.X, -refPt.Y);
				Rotate(th_degg);
				Translation(refPt.X, refPt.Y);
			}
		}
		public class zuma 
		{
			public Bitmap img;
			public int x, y;
		}
		public class Line 
		{
			public PointF St;
			public PointF Ed;
		}
		public class Boll 
		{
			public float X, Y, W, H;
			public int speed,FB,BB;
			public float dix, diy;
			public float Xs,Xe,Ys,Ye,M,ivM;
			public float mt_tifm;
			public float backX,backY;
			public Color color;
		}


		BezierCurve obj = new BezierCurve();
		Transform myzum = new Transform();
		List<zuma> zum = new List<zuma>();
		List<Line> lin = new List<Line>();
		List<Boll> boll = new List<Boll>();
		List<Boll> cervboll = new List<Boll>();
		List<int> PosBollRemove = new List<int>();
		Stack<Boll> Sboll = new Stack<Boll>();


		Bitmap off;
		PointF carpt;
        Timer T = new Timer();
        public Form1()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.Load += Form1_Load;
            this.MouseMove += Form1_MouseMove;
            this.KeyDown += Form1_KeyDown;
            this.Paint += Form1_Paint;
            T.Tick += T_Tick;
			T.Start();
        }


		private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode==Keys.Space) 
			{
				CreateFrogBoll();
				
			}

            if (e.KeyCode == Keys.H) 
			{
				T.Stop();
				for (int i = 0; i < cervboll.Count; i++)
					MessageBox.Show("(" + cervboll[i].FB + "," + cervboll[i].BB + ")  " + i + " = " + cervboll[i].color);

			}
			//DrawDubb(this.CreateGraphics());
		}
		public void CreateFrogBoll() 
		{
			Boll pnn = new Boll();
			Boll x;
			x = Sboll.Pop();
			pnn.X = x.X-65;
			pnn.Y = x.Y;

			pnn.W = x.W;
			pnn.H = x.H;

			pnn.speed = 23;

			pnn.color = x.color;

			SetVallBollFrog(pnn);
			boll.Add(pnn);
			
		}
		public int creatscbllcerv = 0,wave=0,prewave=-1;
        private void T_Tick(object sender, EventArgs e)
        {
            if (Sboll.Count==0) 
			{
				CreateBollOfStack();
			}
			if (cervboll.Count == 0)
			{
				prewave = wave;
				wave++;
			}
			if (cervboll.Count<40 && wave>prewave) 
			{
				creatscbllcerv = 0;
				CreatebollCearve();
                if (cervboll.Count == 40) 
				{
					prewave = wave;
				}
			}
			creatscbllcerv++;
			
			MoveBollOnCearve();
			
			MoveBollFrogDDA();

			ChickIsPosAndColor();

			setcolors();

			DrawDubb(this.CreateGraphics());
		}
		public void setcolors()
		{
			for (int i = 0; i < cervboll.Count; i++)
			{
				cervboll[i].FB = 0; cervboll[i].BB = 0;
				for (int j = i + 1; j < cervboll.Count; j++)
				{
					if (cervboll[i].color == cervboll[j].color)
					{
						cervboll[i].FB++;
					}
					else
					{
						break;
					}
				}
				for (int j = i - 1; j > -1; j--)
				{
					if (cervboll[i].color == cervboll[j].color)
					{
						cervboll[i].BB++;
					}
					else
					{
						break;
					}
				}
			}
		}
		public float catx = 0, caty = 0,ct=0,ct2=0;
		public void ChickIsPosAndColor() 
		{
			for (int i = 0; i < boll.Count; i++)
			{
				ct = 0;
				ct2 = 0;
				for (int j = 0; j < cervboll.Count; j++)
				{
					if(cervboll[j].X >= boll[i].X   || cervboll[j].X + cervboll[j].W >= boll[i].X)
					{
						if (cervboll[j].X <= boll[i].X + boll[i].W || cervboll[j].X + cervboll[j].W <= boll[i].X + boll[i].W)
						{
							if (cervboll[j].Y >= boll[i].Y || cervboll[j].Y + cervboll[j].H >= boll[i].Y) 
							{
								if (cervboll[j].Y <= boll[i].Y + boll[i].H || cervboll[j].Y + cervboll[j].H <= boll[i].Y + boll[i].H)
								{
                                    if (cervboll[j].color == boll[i].color)
                                    {

                                        //for(int k = j+1; k <=j+cervboll[j].FB; k++) 
                                        //{
                                        //	cervboll.RemoveAt(k);
                                        //}
                                        //for (int k = j-1; k >=j-cervboll[j].BB; k--)
                                        //{
                                        //	cervboll.RemoveAt(k);
                                        //}

                                        //if (cervboll[j].BB != 0 || cervboll[j].FB != 0)
                                        //{

                                        //	cervboll.RemoveAt(j);
                                        //	boll.RemoveAt(i);
                                        //	ct2 = 1;

                                        //	MessageBox.Show("Hussam");
                                        //}
                                        //                              else 
                                        //{
                                        //	SwapAndInsertNewBoll(j, i);
                                        //}

                                        //break;

                                        //PosBollRemove.Add(j);
                                        for (int k = j + 1; k < cervboll.Count; k++)
                                        {
                                            if (boll[i].color == cervboll[k].color)
                                            {
                                                PosBollRemove.Add(k);
                                                //cervboll.RemoveAt(k);
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }

                                        for (int k = j - 1; k >= 0; k--)
                                        {
                                            if (boll[i].color == cervboll[k].color)
                                            {
                                                PosBollRemove.Add(k);
                                                //cervboll.RemoveAt(k);
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }

                                        if (PosBollRemove.Count == 0)
                                        {
                                            SwapAndInsertNewBoll(j, i);
                                            //cervboll.Insert(j, boll[i]);
                                        }
                                        else
                                        {
                                            for (int k = 0; k < PosBollRemove.Count; k++)
                                            {
                                                cervboll.Remove(cervboll[PosBollRemove[k]]);
                                            }
                                            cervboll.Remove(cervboll[j]);
                                            boll.Remove(boll[i]);
                                            PosBollRemove.Clear();
											
                                        }
                                        ct = 1;
                                        break;
                                    }
									else 
									{
										if (ct == 0)
										{
											SwapAndInsertNewBoll(j, i);
											break;
										}
									}
                                    
                                }
							}
						}
					}
				}
                if (ct > 0) 
				{
					break;
				}
			}
		}
		public void SwapAndInsertNewBoll(int j,int i) 
		{
			boll[i].X = cervboll[j].X;
			boll[i].Y = cervboll[j].Y;
			boll[i].mt_tifm = cervboll[j].mt_tifm;
			boll[i].dix = 0;
			boll[i].diy = 0;
			boll[i].M = 0;
			boll[i].ivM = 0;
			boll[i].speed = 0;
			boll[i].Xs = 0;
			boll[i].Ys = 0;
			boll[i].Ye = 0;
			boll[i].Xe = 0;

			for (int g = j; g < cervboll.Count; g++)
			{
				if (cervboll[g].mt_tifm < 0.2)
				{
					cervboll[g].mt_tifm -= 0.0035f;
				}
				else if (cervboll[g].mt_tifm > 0.2 && cervboll[g].mt_tifm < 0.7)
				{
					cervboll[g].mt_tifm -= 0.0035f;
				}
				else
				{
					cervboll[g].mt_tifm -= 0.0035f;
				}
				carpt = obj.CalcCurvePointAtTime(cervboll[g].mt_tifm);
				cervboll[g].X = carpt.X;
				cervboll[g].Y = carpt.Y;
			}
			ct++;
			cervboll.Insert(j, boll[i]);
			boll.Remove(boll[i]);
		}
		public void SetVallBollFrog(Boll v) 
		{
			v.Xs=myzum.ps.X;
			v.Ys=myzum.ps.Y;

			v.Xe = ex;
			v.Ye = ey;

			v.dix=v.Xe-v.Xs;
			v.diy=v.Ye-v.Ys;
			v.M=v.diy/v.dix;
			v.ivM=v.dix/v.diy;
		}
        public void MoveBollFrogDDA() 
		{
			for (int i=0;i<boll.Count;i++) 
			{
				if (Math.Abs(boll[i].dix) > Math.Abs(boll[i].diy))
				{
                    if (boll[i].Xs < boll[i].Xe)
                    {
                        boll[i].X += boll[i].speed;
                        boll[i].Y += boll[i].M * boll[i].speed;
                        
                    }
                    else
                    {
                        boll[i].X -= boll[i].speed;
                        boll[i].Y -= boll[i].M * boll[i].speed;
                        
                    }
					
				}
				else
				{
					if (boll[i].Ys < boll[i].Ye)
					{
						boll[i].Y += boll[i].speed;
						boll[i].X += boll[i].ivM * boll[i].speed;
						
					}
					else
					{
						boll[i].Y -= boll[i].speed;
						boll[i].X -= boll[i].ivM * boll[i].speed;
						
					}
				}
			}
		}
		public void MoveBollOnCearve() 
		{
			for (int i = 0; i < cervboll.Count; i++)
			{
				if (cervboll[i].mt_tifm < 1)
				{
					if (cervboll[i].mt_tifm < 0.2)
					{
						cervboll[i].mt_tifm += 0.0035f;
					}
                    else if(cervboll[i].mt_tifm>0.2 ) 
					{
						cervboll[i].mt_tifm += 0.001f;
					}
                   

                    if (i > 0)
                    {
                        ChickIsGoodposXAndY(i - 1, i);
                    }
                    carpt = obj.CalcCurvePointAtTime(cervboll[i].mt_tifm);
                    cervboll[i].X = carpt.X;
                    cervboll[i].Y = carpt.Y;
                    //هنااا تكمن المشكله 
                    //              if (i == 0)
                    //              {
                    //                  cervboll[i].mt_tifm += 0.0035f;
                    //                  carpt = obj.CalcCurvePointAtTime(cervboll[i].mt_tifm);

                    //cervboll[i].backX = cervboll[i].X - (carpt.X - cervboll[i].X);
                    //cervboll[i].backY = cervboll[i].Y - (carpt.Y - cervboll[i].Y);

                    //                  cervboll[i].X = carpt.X;
                    //                  cervboll[i].Y = carpt.Y;
                    //              }
                    //              else
                    //              {
                    //cervboll[i].X = cervboll[i - 1].backX;
                    //cervboll[i].Y = cervboll[i - 1].backY;

                    //cervboll[i].backX = cervboll[i].X - (cervboll[i - 1].backX - cervboll[i].X);
                    //cervboll[i].backY = cervboll[i].Y - (cervboll[i - 1].backY - cervboll[i].Y);
                    //              }
                }
                else
				{
					cervboll.Remove(cervboll[i]);
				}
			}
		}
		public void ChickIsGoodposXAndY(int old,int add) 
		{
			if (cervboll[old].X > cervboll[add].X)
			{
				cervboll[add].mt_tifm = cervboll[old].mt_tifm - 0.0035f;
			}
		}
		double rad = 0, deg = 0;
		float ex = 0,ey=0;
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
			ex = e.X;
			ey = e.Y;
			
			int dx = e.X - (int)myzum.ps.X;
			int dy = e.Y - (int)myzum.ps.Y;

			rad = Math.Atan2(dy, dx);
			deg = (rad * 180) / Math.PI;

			myzum.image = zum[0].img;
			myzum.image = RotateImage(myzum.image, (float)deg -270);
			//DrawDubb(this.CreateGraphics());
		}
		public Bitmap RotateImage(Bitmap bitmap,float angle) 
		{
			Bitmap returnBitmap = new Bitmap(bitmap.Width, bitmap.Height);
			Graphics graphics = Graphics.FromImage(returnBitmap);
			graphics.TranslateTransform((float)bitmap.Width / 2, (float)bitmap.Height / 2);
			graphics.RotateTransform(angle);
			graphics.TranslateTransform(-(float)bitmap.Width / 2, -(float)bitmap.Height / 2);
			graphics.DrawImage(bitmap, new Point(0, 0));
			return returnBitmap;
		}
		private void Form1_Paint(object sender, PaintEventArgs e)
        {
            DrawDubb(e.Graphics);
        }
		private void Form1_Load(object sender, EventArgs e)
		{
			off = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
			CreateMap();
			CreateZuma();

			////CreateLine();Hide line 1
			////CreateBollOfStack();
			////CreatebollCearve();
			////DrawDubb(this.CreateGraphics());
		}
		public void CreatebollCearve() 
		{
			Color[] x = { Color.Yellow, Color.White, Color.Blue, Color.Green };
			Boll pnn = new Boll();
			pnn.X = obj.ControlPoints[0].X;
			pnn.H = 30;
			pnn.W = 30;
			pnn.Y = obj.ControlPoints[0].Y;
			Random rr = new Random();
			pnn.color = x[rr.Next(4)];
			pnn.mt_tifm = 0.0f; 
			pnn.speed = 1;
			cervboll.Add(pnn);
		}
		public void CreateBollOfStack()
		{
			Color[] x = {Color.Green,Color.Blue,Color.White,Color.Yellow};
			Boll pnn = new Boll();
			pnn.X = this.ClientSize.Width / 2+100;
			pnn.H = 30;
			pnn.W = 30;
			pnn.Y = this.ClientSize.Height-pnn.H-10;
			Random rr = new Random();
			pnn.color = x[rr.Next(4)]; //Color.FromArgb(rr.Next(1,225),rr.Next(1,225),rr.Next(1,255)); //
			Sboll.Push(pnn);
		}
		public void CreateLine() 
		{
			int x = 270;
			for(int i = 0; i < 4; i++) 
			{
				Line pnn = new Line();
				pnn.St.X =ex;
				pnn.St.Y =ey;
				PointF p = Getnextpoint(x);
				pnn.Ed.X =p.X+25;
				pnn.Ed.Y =this.ClientSize.Height-zum[0].img.Height+170;

				lin.Add(pnn);
				x += 3;
			}
		}//Hide line 2
		public PointF Getnextpoint(int theta)
		{

			PointF p = new PointF();
			float thRadian = (float)(theta * Math.PI / 180);
			p.X = (float)(100*Math.Cos(thRadian))+zum[0].x;
			p.Y = (float)(100*Math.Sin(thRadian));
			return p;
		}
		public void CreateZuma() 
		{
			zuma pnn = new zuma();
			pnn.x = this.ClientSize.Width / 2;
			pnn.img = new Bitmap("act.png");
			pnn.y = this.ClientSize.Height-pnn.img.Height+160;
			myzum.image = pnn.img;
			myzum.ps.X = pnn.x;
			myzum.ps.Y = pnn.y;
			zum.Add(pnn);
		}
		public void CreateMap() 
		{
			//int[] x = { 70, 90,120,   400,800,1200,1500,   1500,1500,1500,     1600,1500,1400,1300,1000,800,500,300,100,  100,100,100,  200,300,400,500,600,700 };
			//int[] y = { 0,75,150,     100,100,100,100,     90,150,230,         230,230,230,230,230,230,230,230,230,       220,280,360,  360,360,360,360,360,360 };

			int[] x = { 70, 90, 120 };
			int[] y = { 0, 75, 150 };



			for (int i = 0; i < x.Length; i++)
			{
				obj.SetControlPoint(new Point(x[i], y[i]));
			}
			int v = 120;
			for (int i = 0; i < 4; i++)
			{
				v = v + 440;
				obj.SetControlPoint(new Point(v, y[2]));

			}


			int m = 0;
			for (int i = 0; i < 3; i++)
			{
				m = m + 100;
				obj.SetControlPoint(new Point(v, m));
			}
			for (int i = 0; i < 4; i++)
			{
				v = v - 580;
				obj.SetControlPoint(new Point(v, m));
			}

			
			m -= 150;
			for (int i = 0; i < 3; i++)
			{
				m = m + 100;
				obj.SetControlPoint(new Point(v, m));
			}
			for (int i = 0; i < 5; i++)
			{
				v = v + 450;
				obj.SetControlPoint(new Point(v, m));
			}



			m -= 150;
			for (int i = 0; i < 3; i++)
			{
				m = m + 100;
				obj.SetControlPoint(new Point(v, m));
			}
			for (int i = 0; i < 4; i++)
			{
				v = v - 440;
				obj.SetControlPoint(new Point(v, m));
			}


			for (int i = 0; i < 1; i++)
			{
				m = m + 150;
				obj.SetControlPoint(new Point(v, m));
			}
			
		}
        


        void DrawDubb(Graphics g)
        {
            Graphics g2 = Graphics.FromImage(off);
            DrawScene(g2);
            g.DrawImage(off, 0, 0);
        }
        private void DrawScene(Graphics g)
        {
            g.Clear(Color.Black);
			g.DrawRectangle(Pens.White, obj.ControlPoints[obj.ControlPoints.Count - 1].X - 40, obj.ControlPoints[obj.ControlPoints.Count - 1].Y - 40, 80, 80);
			obj.DrawCurve(g);
			g.DrawImage(myzum.image, myzum.ps.X, myzum.ps.Y, 100, 100);
			myzum.image.MakeTransparent(myzum.image.GetPixel(0, 0));
            for (int i=0;i<lin.Count;i++) 
			{
				g.DrawLine(Pens.White,lin[i].St.X, lin[i].St.Y, lin[i].Ed.X, lin[i].Ed.Y);
			}

			//g.DrawLine(Pens.White,myzum.ps.X , myzum.ps.Y, ex, ey);
			for (int i = 0; i < Sboll.Count; i++)
            {
                SolidBrush s = new SolidBrush(Sboll.Peek().color);
                g.FillEllipse(s, Sboll.Peek().X, Sboll.Peek().Y, Sboll.Peek().W, Sboll.Peek().H);
                
            }

            for (int i = 0; i < boll.Count; i++)
			{
				SolidBrush s = new SolidBrush(boll[i].color);
				g.FillEllipse(s, boll[i].X, boll[i].Y, boll[i].W, boll[i].H);
				Font Ft = new Font("System", 15);
				g.DrawString(""+"***", Ft, new SolidBrush(Color.Gray), boll[i].X - 10, boll[i].Y - 8);
			}

			for (int i = 0; i < cervboll.Count; i++)
			{
				SolidBrush s = new SolidBrush(cervboll[i].color);
				
				g.FillEllipse(s, cervboll[i].X-15, cervboll[i].Y-15, cervboll[i].W, cervboll[i].H);
                Font Ft = new Font("System", 15);
                g.DrawString("" + i, Ft, new SolidBrush(Color.Gray), cervboll[i].X - 10, cervboll[i].Y - 8);

            }
		}
    }
}
