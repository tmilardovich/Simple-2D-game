using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OTTER
{
    public abstract class Likovi:Sprite
    {
        protected int brzina;

        public int Brzina
        {
            get { return brzina; }
            set { brzina = value; }
        }

        


        public Likovi(string slika, int xcor, int ycor, int brzina)
            : base(slika, xcor, ycor)
        {
            this.brzina = brzina;
        }

    }
    public class Planet : Likovi
    {
        protected int odgovor;

        public int Odgovor
        {
            get { return odgovor; }
            set { odgovor = value; }
        }

        public Planet(string slika, int xcor, int ycor, int brzina, int odgovor)
            : base(slika, xcor, ycor, brzina)
        {
            this.odgovor = odgovor;
        }
    }

    

    public class Raketa : Likovi
    {

        

        public Raketa(string slika, int xcor, int ycor, int brzina)
            : base(slika, xcor, ycor, brzina)
        {

        }

    }
}
