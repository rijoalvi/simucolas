using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace SimSistemaColasSinPrioridadAdquirida
{
    public class Simulador
    {
        public int valorInfinito=9999999;
        public Double[] lambda;//λ=llegadas por minuto, 1/λ=tiempoe entre llegadas
        public Double mu;//cantidad de clientes que se atienden por unidad tiempo,1/μ= tiempo entre llegas
        public enum Servidor { desocupado = -1, ocupadoPorCliente0 = 0, ocupadoPorCliente1 = 1, ocupadoPorCliente2 = 2 };
        public Double TM;// = hora de la simulación
        /// <summary>
        /// Tiempo programado para la siguiente llegada (arrival time)
        /// </summary>
        public Double[] AT;
        /// <summary>
        /// Tiempo programada para la siguiente salida (departure time)
        /// </summary>
        public Double[] DT;
        /// <summary>
        /// Estado del Servidor (-1= desocupado, 0= ocupado por cliente tipo 0, 1= ocupado por cliente tipo 1, 2= ocupado por cliente tipo 2)
        /// </summary>
        public int SS;
        /// <summary>
        /// Longitud, en unidades de tiempo, de una corrida de simulación
        /// </summary>
        public int MX;
        /// <summary>
        /// Tiempo entre llegadas
        /// </summary>
        public Double[] IT;
        public Double rho;
        public Queue[] clientesEnColaTipo;
        public int[] WL;
        /// <summary>
        ///  Va enumerando clientes en forma consecutiva no importa su tipo
        /// </summary>
        public int numeroClienteGeneral;
        /// <summary>
        /// Arreglo que con contadores de cada tipo de clientes. Va enumerando clientes en forma consecutiva según su tipo.
        /// </summary>
        public int[] numeroCliente;

        /// <summary>
        /// Tiempo esperado en la cola para cada cliente, calculo con el simulador
        /// </summary>
        public Double[] Wq;

        //variables malas
        public int clientesColas;

        Reportador reportador;
        public int contadorEventos;
        Queue q = new Queue();
        Queue colaClientes = new Queue();
        public List<Cliente> historialClientes;
        public Random r;

        public Estadisticador estadisticador;

       //Double lambdaBuena;


        /*public Simulador()
        {
            r = new Random();
            inicializarVariables();
            historialClientes = new List<Cliente>();
            estadisticador = new Estadisticador();
            
            TM = 0;

            SS = -1;

            MX = 15;

            reportador = new Reportador("reporte1.txt");
        }*/
        public Simulador(Double lambda0,Double lambda1,Double lambda2,Double mu,int MX)
        {
            r = new Random();
            inicializarVariables(lambda0,lambda1,lambda2,mu);
            historialClientes = new List<Cliente>();
            estadisticador = new Estadisticador();

            TM = 0;

            SS = -1;

            this.MX = MX;

            reportador = new Reportador("reporte1.txt");
        }
        public void inicializarVariables() {
            lambda = new Double[3];
            lambda[0] = 1;
            lambda[1] = 1;
            lambda[2] = 1;
            numeroCliente = new int[3];
            mu = .5;
            AT = new Double[3];
            AT[0] = 0;
            AT[1] = generarIT(1);
            AT[2] = generarIT(2);
            DT = new Double[3];
            DT[0] = 9999999;
            DT[1] = 9999999;
            DT[2] = 9999999;
            IT = new Double[3];
            WL = new int[3];
            clientesEnColaTipo=new Queue[3];
            clientesEnColaTipo[0] = new Queue();
            clientesEnColaTipo[1] = new Queue();
            clientesEnColaTipo[2] = new Queue();
        }
        public void inicializarVariables(Double lambda0, Double lambda1, Double lambda2, Double mu)
        {
            lambda = new Double[3];
            lambda[0] = lambda0;
            lambda[1] = lambda1;
            lambda[2] = lambda2;
            numeroCliente = new int[3];
            this.mu = mu;
            AT = new Double[3];
            AT[0] = 0.0001;
            AT[1] = generarIT(1);
            AT[2] = generarIT(2);
            DT = new Double[3];
            DT[0] = 9999999;
            DT[1] = 9999999;
            DT[2] = 9999999;
            IT = new Double[3];
            WL = new int[3];
            clientesEnColaTipo = new Queue[3];
            clientesEnColaTipo[0] = new Queue();
            clientesEnColaTipo[1] = new Queue();
            clientesEnColaTipo[2] = new Queue();
            Wq = new Double[3];
            Wq[0] = 0;
            Wq[1] = 0;
            Wq[2] = 0;
        }
        public void correr() {

            while(TM<=MX){
                contadorEventos++;

                if (AT[min(AT)] < DT[min(DT)])//si llega alguien antes que salga el siguiente
                {//procesar llegada
                    int indiceTipoCliente = min(AT);//selecciono el cliente con su Arrival Time menor (el que va a llegar primero)

                    procesarUnaLlegada(indiceTipoCliente);


                }
                else { //procesar salida

                    procesarUnaSalida();


                }


            }

            for (int i = 0; i < estadisticador.dominioMiliSegundo.Count;i++ )
            {
                Wq[0] = Wq[0] + estadisticador.dominioMiliSegundo[i].Wq[0];
                Wq[1] = Wq[1] + estadisticador.dominioMiliSegundo[i].Wq[1];
                Wq[2] = Wq[2] + estadisticador.dominioMiliSegundo[i].Wq[2];
            }
            Wq[0] = Wq[0] / estadisticador.dominioMiliSegundo.Count;
            Wq[1] = Wq[1] / estadisticador.dominioMiliSegundo.Count;
            Wq[2] = Wq[2] / estadisticador.dominioMiliSegundo.Count;

            reportador.close();


        }
        public void procesarUnaLlegada(int tipoCliente)
        {
 
            numeroClienteGeneral++;
            numeroCliente[tipoCliente]++;
            TM = AT[tipoCliente];

            if ((int)Servidor.desocupado == SS)
            {//servidor libre
                SS = tipoCliente;//se establece servidor ocupado por cierto tipo cliente
                //Generar ST
                DT[tipoCliente] = TM + generarST();
                Cliente clienteAEncolarse = new Cliente(numeroCliente[tipoCliente], tipoCliente, TM, numeroClienteGeneral);
                clientesEnColaTipo[tipoCliente].Enqueue(clienteAEncolarse);

            }
            else
            {//Servidor ocupado

                WL[tipoCliente] = WL[tipoCliente] + 1;

                clientesColas = clientesColas + 1;//no recuerdo que es
                q.Enqueue(numeroCliente);
                Cliente clienteAEncolarse=new Cliente(numeroCliente[tipoCliente],tipoCliente,TM,numeroClienteGeneral);
                clientesEnColaTipo[tipoCliente].Enqueue(clienteAEncolarse);
      

            }
            //Generar IT
            AT[tipoCliente] = TM + generarIT(tipoCliente);
            
            escribirNuevoEvento(contadorEventos, "Llegad", numeroClienteGeneral, tipoCliente, numeroCliente[tipoCliente], TM, SS, WL, AT, DT);
        }
        public void procesarUnaSalida() {
            for (int i = 0; i < 3; i++ )
            {
                TM = DT[SS];//TM igual a la salida del cliente ocupando servidor
                if (WL[i] > 0)//si hay cliente tipo 0 esperando en cola
                {


                    //TM = DT[SS];//TM igual a la salida del cliente ocupando servidor
                    int elQueVaSaliendo = SS;
                    DT[SS] = valorInfinito;//porque si otro tipo estaba el el servicio, el siguiente del mismo tipo va a seguir en la cola, si se atiende otro con más pririodad (por ende no se le asigna DT)
                    SS = i;//ahora el servidor lo tiene este tipo
                    WL[i]--;

                    DT[i] = TM + generarST();//establecer el DT del siguiente

                    //escribirNuevoEvento(contadorEventos, "salida", numeroClienteGeneral, i, numeroCliente[i], TM, SS, WL, AT, DT);
                    Cliente clienteASalir = (Cliente)clientesEnColaTipo[elQueVaSaliendo].Dequeue();
                    escribirNuevoEvento(contadorEventos, "salida", clienteASalir.IDGeneral, elQueVaSaliendo, clienteASalir.ID, TM, SS, WL, AT, DT);

                    return;
                }
                else if (valorInfinito != DT[i])
                {//si llegó cliente tipo i y como no hay cola, entra directo.
                    int elQueVaSaliendo = SS;
                    SS = (int)Servidor.desocupado;
                    DT[i] = valorInfinito;//El departure time del siguiente es infinito, por que acaba de termiar el último de este tipo
                    Cliente clienteASalir = (Cliente)clientesEnColaTipo[elQueVaSaliendo].Dequeue();
                    escribirNuevoEvento(contadorEventos, "salida", clienteASalir.IDGeneral, elQueVaSaliendo, clienteASalir.ID, TM, SS, WL, AT, DT);
                    //escribirNuevoEvento(contadorEventos, "salida", ((Cliente)clientesEnColaTipo[i].Dequeue()).IDGeneral, i, ((Cliente)clientesEnColaTipo[i].Dequeue()).ID, TM, SS, WL, AT, DT);


                    for (int j = i+1; j < 3; j++)
                    {
                        if(WL[j]>0){
                            SS = j;
                            WL[j]--;
                            DT[j] = TM + generarST();
                            reportador.controlServidor(-1, j, -1, TM, DT[j]);
                            return;
                        }
                    }
                    return; // termino de procesarse la salida
                }
            }
            MessageBox.Show("Error procesando una salida");//No se espera que se llege hasta acá.
        }

        public void escribirNuevoEvento(int contadorEventos, String tipo, int numeroClienteGeneral,  int tipoCliente,int numeroClienteTipo, Double TM, int SS, int[] WL, Double[] AT, Double[] DT){
            
            reportador.escribirNuevoEvento(contadorEventos,         tipo,     numeroClienteGeneral,  tipoCliente,numeroClienteTipo,            TM,        SS,    WL,        AT,        DT);

            estadisticador.ingresarMilisegundo(TM, WL, SS);
           // estadisticador.ingresarMinuto(TM, WL, SS);
        }
        private Double generarST(){
           
            Double Ws = 1 / mu;//Ws=tiempo promedio de un cliente en el servicio
            return Ws;
        }

        private Double generarIT(int tipoCliente)//           1/λ
        {
            Double valor = 0;
            //Double rand = (r.Next(0, 10099000) / 10000000.0);
            Double promedio=0;
            for (int i = 0; i <100; i++)
            {
                //Double rand = (r.Next(0, 10000000) / 10000000.0);
                Double rand = r.NextDouble();
                if (rand == 0)
                {
                    //rand = 0.000000001;
                    rand = 0.001;
                }
                valor = (Math.Log(rand) / (lambda[tipoCliente] * -1));
                promedio += valor;
            }
            promedio = promedio / 100;

            return promedio;
        }
        public Double calcularTiempoEnCola(){
            Double tiempoPromedioEnCola = 0;
            for (int i = 0; i<historialClientes.Count; i++ )
            {
                tiempoPromedioEnCola = tiempoPromedioEnCola+historialClientes[i].tiempoEsperaCola;
            }
            tiempoPromedioEnCola = tiempoPromedioEnCola / historialClientes.Count;
            return tiempoPromedioEnCola;
        }
        /// <summary>
        /// Calcula el elemento mínimo del vector
        /// </summary>
        /// <param name="x">El índice del elemento menor del arreglo</param>
        /// <returns></returns>
        private int min(Double[] x)
        {

            if ((x[0] <= x[1]) && (x[0] <= x[2]))
            {
                return 0;
            }
            else if ((x[1] <= x[0]) && (x[1] <= x[2]))
            {
                return 1;
            }
            else if ((x[2] <= x[0]) && (x[2] <= x[1]))
            {
                return 2;
            }
            MessageBox.Show("Error en calcularMinimo");
            return -1;
        }
    }
}
