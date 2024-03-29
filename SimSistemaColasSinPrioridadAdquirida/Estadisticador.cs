﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimSistemaColasSinPrioridadAdquirida
{
    /// <summary>
    /// Es el elemento que realiza los cálculos de tiempos entre eventos del sistema. Cada evento que sucede es reportado a este, y se encarga de hacer los cálculos y poder ponderar los tiempos. Al final del programa se reportan en la pantalla principal del programa.
    /// </summary>
    public class Estadisticador
    {
        public List<Minuto> dominioTiempo;
        public List<MiliSegundo> dominioMiliSegundo;
        public int contadorTiempo;
        public Estadisticador() {
            dominioTiempo = new List<Minuto>();
            dominioMiliSegundo = new List<MiliSegundo>();
            contadorTiempo=-1;
        }
        public void ingresarMinuto(Double TM, int WL, int SS){
            if (contadorTiempo == (TM - 1))
            {
                dominioTiempo.Add(new Minuto(TM, WL,SS));
                contadorTiempo++;
            }
            else {
                for (int i = (int)dominioTiempo[dominioTiempo.Count - 1].TM+1; i < TM; i++)//Estrapolación
                {
                    dominioTiempo.Add(new Minuto(i, dominioTiempo[dominioTiempo.Count-1].WL,SS));
                }
                dominioTiempo.Add(new Minuto(TM, WL,SS));
            }
        
        }
        public void ingresarMilisegundo(Double TM, int[] WL, int SS)
        {
            int milisegundoActual =(int)(TM * 1000);
            if (contadorTiempo == (milisegundoActual - 1))
            {
                dominioMiliSegundo.Add(new MiliSegundo(milisegundoActual, WL, SS));
                contadorTiempo++;
            }
            else
            {
                int inicio = ((int)dominioMiliSegundo[dominioMiliSegundo.Count - 1].TM);
                for (int i = (inicio + 1); i < milisegundoActual; i++)//interpolación
                {
                    dominioMiliSegundo.Add(new MiliSegundo(i, dominioMiliSegundo[dominioMiliSegundo.Count - 1].Lq, dominioMiliSegundo[dominioMiliSegundo.Count - 1].SS));
                }
                dominioMiliSegundo.Add(new MiliSegundo(milisegundoActual, WL, SS));
            }

        }
    }
}
