using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace platformerGame.Utilities
{
    class MyRect<T> where T : struct
    {
        public T Left { get; set; }
        public T Top { get; set; }
        public T Width { get; set; }
        public T Height { get; set; }

        public MyRect()
        {
            Left = Top = Width = Height = default(T);
        }

        public MyRect(T left, T top, T w, T h)
        {
            this.Left = left;
            this.Top = top;
            this.Width = w;
            this.Height = h;
        }

        public MyRect(MyRect<T> other) : this(other.Left,other.Top,other.Width,other.Height)
        {
        }

        public MyRect<T> DeepCopy()
        {
            return new MyRect<T>(this);
        }

        public MyRect<T> ShallowCopy()
        {
            return (MyRect<T>)this.MemberwiseClone();
        }
    }

    class MyIntRect : MyRect<int>
    {
        
        public MyIntRect() : base()
        {
        }

        public MyIntRect(int left, int top, int w, int h) : base(left,top,w,h)
        {
        }

        public MyIntRect(MyIntRect other) : base(other.Left, other.Top, other.Width, other.Height)
        {
        }

        public new MyIntRect DeepCopy()
        {
            return new MyIntRect(this);
        }

        public IntRect AsSfmlIntRect()
        {
            return new IntRect(Left, Top, Width, Height);
        }
        
    }
    
    
    class MyFloatRect : MyRect<float>
    {
        public MyFloatRect() : base()
        {
        }

        public MyFloatRect(float left, float top, float w, float h) : base(left, top, w, h)
        {
        }

        public MyFloatRect(MyFloatRect other) : base(other.Left, other.Top, other.Width, other.Height)
        {
        }

        public new MyFloatRect DeepCopy()
        {
            return new MyFloatRect(this);
        }

        public FloatRect AsSfmlFloatRect()
        {
            return new FloatRect(Left, Top, Width, Height);
        }
    }
    
}
