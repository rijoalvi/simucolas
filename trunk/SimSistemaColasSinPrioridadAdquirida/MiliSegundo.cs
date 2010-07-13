using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimSistemaColasSinPrioridadAdquirida
{
    /// <summary>
    /// Es un elemento que usamos para grabar el estado en un instante del sistema. Contiene el milisegundo que marca el sistema, el estado de la cola, y el estado del servidor.  Es utilizado por el estadisticador para llevar el control de los eventos en el sistema.
    /// </summary>
    public class MiliSegundo
    {
        public int TM;
        public int[] Lq;
        
        public int SS;
        public MiliSegundo(int TM, int[] WL, int SS)
        {
            this.TM = TM;
            this.Lq = new int[3];
            this.Lq[0] = WL[0];
            this.Lq[1] = WL[1];
            this.Lq[2] = WL[2];
            this.SS = SS;
        }
        public override string ToString()
        {
            return "(TM," + TM + ")(WL1," + Lq[0] + ")(WL2," + Lq[1] + ")(WL3," + Lq[2] + ")" + "(SS," + SS + ")";
        }
    }
}
