using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimSistemaColasSinPrioridadAdquirida
{
    public class MiliSegundo
    {
        public int TM;
        public int[] Wq;
        
        public int SS;
        public MiliSegundo(int TM, int[] WL, int SS)
        {
            this.TM = TM;
            this.Wq = new int[3];
            this.Wq[0] = WL[0];
            this.Wq[1] = WL[1];
            this.Wq[2] = WL[2];
            this.SS = SS;
        }
        public override string ToString()
        {
            return "(TM," + TM + ")(WL1," + Wq[0] + ")(WL2," + Wq[1] + ")(WL3," + Wq[2] + ")" + "(SS," + SS + ")";
        }
    }
}
