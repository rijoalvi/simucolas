using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SimSistemaColasSinPrioridadAdquirida
{
    class Reportador
    {
        public TextWriter file;

        public Reportador(String fileName)
        {
            file = new StreamWriter(fileName);
            file.WriteLine("Final del Evento\t" +
                " Tipo de Evento\t\t" +
                " Cliente Número General\t\t" +
                " Tipo Cliente\t\t" +
                " Número Cliente (Tipo)\t\t" +
                " TM\t\t" +
                " SS\t\t" +
                " WL0\t\t" +
                " WL1\t\t" +
                " WL2\t\t" +
                " AT0\t\t" +
                " AT1\t\t" +
                " AT2 (next)\t\t" +
                " DT0\t\t" +
                " DT1\t\t" + 
                " DT2");
        }
        public void escribirNuevoEvento
                                    (int numeroEvento, 
                                    String tipoEvento, 
                                    int clienteNumeroGeneral,
                                    int tipoCliente,
                                    int clienteNumeroTipo, 
                                    Double TM, 
                                    int SS, 
                                    int[] WL, 
                                    Double[] AT,
                                    Double[] DT
                                    )
        {
            String at0 = AT[0].ToString().Substring(0, 5);
            String at1 = AT[1].ToString().Substring(0, 5);
            String at2 = AT[2].ToString().Substring(0, 5);
            if (0==tipoCliente)
            {
                at1 = "-";
                at2 = "-";
            }
            if (1 == tipoCliente)
            {
                at0 = "-";
                at2 = "-";
            }
            if (2 == tipoCliente)
            {
                at0 = "-";
                at1 = "-";
            }
            //String  TMString="";
            /*if(TM==0){
                TMString="0";

            }else{
                TMString = TM.ToString().Substring(0, 5);
            }*/
            file.WriteLine(
                numeroEvento + "\t\t\t" +
                tipoEvento + "(" + tipoCliente + ")\t\t\t" +
                clienteNumeroGeneral + "\t\t\t\t" +
                tipoCliente + "\t\t\t" +
                clienteNumeroTipo + "\t\t\t" +
                TM.ToString().Substring(0, 6) + "\t\t" +
                SS + "\t\t" +
                WL[0] + "\t\t" +
                WL[1] + "\t\t" +
                WL[2] + "\t\t" +
                at0 + "\t\t" +
                at1 + "\t\t" +
                at2 + "\t\t\t" +
                DT[0].ToString().Substring(0, 6) + "\t\t" +
                DT[1].ToString().Substring(0, 6) + "\t\t" +
                DT[2].ToString().Substring(0, 6) + "\t\t" 
                );
        }
        public void controlServidor
                                    (
                                    int clienteNumeroGeneral,
                                    int tipoCliente,
                                    int clienteNumeroTipo,
                                    Double TM,
                                    Double DT
                                    )
        {
            file.WriteLine("* Cliente " + clienteNumeroGeneral + " (" + tipoCliente + ") tomó servidor en el tiempo " + TM + " y se le asigno DT " + DT);
        }
        public void close()
        {
            file.Close();
        }
    }
}
