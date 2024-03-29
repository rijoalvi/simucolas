﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SimSistemaColasSinPrioridadAdquirida
{
    public partial class FormResultados : Form
    {
        public Simulador simulador;
        public FormResultados()
        {
            InitializeComponent();
            hacerCalculos();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //simulador = new Simulador(Double.Parse(lambda1TextBox.Text),Double.Parse(lambda2TextBox.Text),Double.Parse(lambda3TextBox.Text),Double.Parse(muTextBox.Text),Int32.Parse(MXTextBox.Text));
            hacerCalculosTiempoEntreLLegadas();
            calcularMu();

        }
        public void hacerCalculos()
        {
            try
            {
                mensajeLabel.Text = "";
                mensajesRhosLabel.Text = "";
                //tiempoEsperadoServicioTextBox.Text = (1 / Double.Parse(muTextBox.Text)).ToString();
                tiempoEsperadoServicioCuadradoTextBox.Text = (1 / (Math.Pow(Double.Parse(muTextBox.Text),2))).ToString();
                Double lambda1 = Double.Parse(lambda1TextBox.Text);
                Double lambda2 = Double.Parse(lambda2TextBox.Text);
                Double lambda3 = Double.Parse(lambda3TextBox.Text);
                Double mu = Double.Parse(muTextBox.Text);
                Double tiempoEsperadoServicioCuadrado = Double.Parse(tiempoEsperadoServicioCuadradoTextBox.Text);
                Double rho1=lambda1 /mu;
                Double rho2 = lambda2 /mu;
                Double rho3 = lambda3 / mu;
                Double a0=0;
                Double a1=rho1;
                Double a2 = rho1 + rho2;
                Double a3 = rho1 + rho2 + rho3;

                Double numeradorFormula = ((lambda1 * tiempoEsperadoServicioCuadrado) / 2) + ((lambda2 * tiempoEsperadoServicioCuadrado) / 2) + ((lambda3 * tiempoEsperadoServicioCuadrado) / 2);
   

                Double Wq1 = numeradorFormula/
                             ((1 - a0) * (1 - a1));
                Double Wq2 = numeradorFormula /
                             ((1 - a1) * (1 - a2));
                Double Wq3 = numeradorFormula/
                             ((1 - a2) * (1 - a3));
                rho1TextBox.Text = rho1.ToString();
                rho2TextBox.Text = rho2.ToString();
                rho3TextBox.Text = rho3.ToString();
                Wq1TextBox.Text = Wq1.ToString();
                Wq2TextBox.Text = Wq2.ToString();
                Wq3TextBox.Text = Wq3.ToString();
                Lq1TextBox.Text = (lambda1 * Wq1).ToString();
                Lq2TextBox.Text = (lambda2 * Wq2).ToString();
                Lq3TextBox.Text = (lambda3 * Wq3).ToString();
                Double sumaRhos = rho1 + rho2 + rho3;
                sumaRhosTextBox.Text = sumaRhos.ToString();
                if (sumaRhos < 1)
                {
                    mensajesRhosLabel.ForeColor = Color.MediumBlue;
                    mensajesRhosLabel.Text = "como ρ1+ρ2+ρ3 < 1 existirá un estado estable";
                }
                else {
                    mensajesRhosLabel.ForeColor = Color.Firebrick;
                    mensajesRhosLabel.Text = "como ρ1+ρ2+ρ3 >= 1 no se alcanzará un estado estable";
                }
                if ((Double.Parse(rho1TextBox.Text) >= 1) || (Double.Parse(rho2TextBox.Text) >= 1) || (Double.Parse(rho3TextBox.Text) >= 1))
                {
                    mensajeLabel.Text = "Advertencia: El sistema va a colapsar porque algún rho es mayor a 1";


                }
                calcularWs();


                correrSimulacionButton.Enabled = true;
            }
            catch (Exception e) {

                correrSimulacionButton.Enabled = false;
                mensajeLabel.Text = "Datos no válidos: "+e.Data;
            }
        }
        public void calcularWs() { //calcular los tiempos
         //   WsTextBox.Text = (1 / Double.Parse(muTextBox.Text)).ToString();
        }
        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void lambda1TextBox_TextChanged(object sender, EventArgs e)
        {
            hacerCalculos();
        }

        private void lambda2TextBox_TextChanged(object sender, EventArgs e)
        {
            hacerCalculos();
        }

        private void lambda3TextBox_TextChanged(object sender, EventArgs e)
        {
            hacerCalculos();
        }

        private void muTextBox_TextChanged(object sender, EventArgs e)
        {
            hacerCalculos();
        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void modeloAnaliticoGroupBox_Enter(object sender, EventArgs e)
        {

        }

        private void correrSimulacionButton_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            simulador = new Simulador(Double.Parse(lambda1TextBox.Text), Double.Parse(lambda2TextBox.Text), Double.Parse(lambda3TextBox.Text), Double.Parse(muTextBox.Text), Int32.Parse(MXTextBox.Text));
            simulador.correr();
            Lq1DiscretoTextBox.Text = simulador.Lq[0].ToString();
            Lq2DiscretoTextBox.Text = simulador.Lq[1].ToString();
            Lq3DiscretoTextBox.Text = simulador.Lq[2].ToString();
            
            Wq1DiscretoTextBox.Text = simulador.Wq[0].ToString();
            Wq2DiscretoTextBox.Text = simulador.Wq[1].ToString();
            Wq3DiscretoTextBox.Text = simulador.Wq[2].ToString();



            this.Cursor = Cursors.Default;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            hacerCalculosTiempoEntreLLegadas();
        }

        private void tipo1TextBox_TextChanged(object sender, EventArgs e)
        {
            hacerCalculosTiempoEntreLLegadas();
        }
        private void hacerCalculosTiempoEntreLLegadas() {
            try
            {
                mensajeLabel.Text = "";
                Double tiempoEntreLlegadasTipo1 = Double.Parse(tipo1TextBox.Text);
                Double tiempoEntreLlegadasTipo2 = Double.Parse(tipo2TextBox.Text);
                Double tiempoEntreLlegadasTipo3 = Double.Parse(tipo3TextBox.Text);
                lambda1TextBox.Text = (1 / tiempoEntreLlegadasTipo1).ToString();
                lambda2TextBox.Text = (1 / tiempoEntreLlegadasTipo2).ToString();
                lambda3TextBox.Text = (1 / tiempoEntreLlegadasTipo3).ToString();
            }
            catch (Exception ee)
            {
                mensajeLabel.Text = "Datos inválidos";
            }
        }

        private void tipo2TextBox_TextChanged(object sender, EventArgs e)
        {
            hacerCalculosTiempoEntreLLegadas();
        }

        private void tipo3TextBox_TextChanged(object sender, EventArgs e)
        {
            hacerCalculosTiempoEntreLLegadas();
        }

        private void tiempoEsperadoServicioTextBox_TextChanged(object sender, EventArgs e)
        {
            calcularMu();
        }
        private void calcularMu() {
            try
            {
                muTextBox.Text = (1 / Double.Parse(tiempoEsperadoServicioTextBox.Text)).ToString();
            }
            catch (Exception eee)
            {
                mensajeLabel.Text = "Datos no válidos";
            }
        }

    }
}
