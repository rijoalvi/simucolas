using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SimSistemaColasSinPrioridadAdquirida
{
    /// <summary>
    /// Representa un cliente en el sistema. Con tiene los datos propios de su hora de llegada, hora de salida, el tiempo que espero en la cola, el tipo de cliente que es, y un identificador único.
    /// </summary>
    public class Cliente
    {
        public int IDGeneral;
        public int ID;
        public int tipo;
        public Double TMInicio;
        private Double TMFinal;
        public Double tiempoEsperaCola;
        public Cliente(int ID, int tipo, Double TMInicio, int IDGeneral) {
            this.IDGeneral = IDGeneral;
            this.ID = ID;
            this.tipo = tipo;
            this.TMInicio = TMInicio;
        }
        /// <summary>
        /// Establece el tiempo que el cliente salió de la cola, y calcula el tiempo que estuvo en la cola
        /// </summary>
        /// <param name="TMFinal">The TM final.</param>
        public void setTMFinal(Double TMFinal) { 
            this.TMFinal=TMFinal;
            this.tiempoEsperaCola = TMFinal - TMInicio;
            if (tiempoEsperaCola<=0)
            {
                MessageBox.Show("Error en tiempo en cola");
            }
        }
    }
}
