using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUSim.Source
{
  public class Dados
    {

       
        public static byte[] data = new byte[256];
        public static int address;
        private static Dados instance;

        private Dados(){
            
            
            }
        public static Dados Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Dados();
                }
                return instance;
            }
        }
        public static void endereco(int end, byte dado) {
           
            data[end] = dado;
        }


    }
}
