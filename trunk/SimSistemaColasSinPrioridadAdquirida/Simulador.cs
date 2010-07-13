using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//Luis Carlos Chavarría 
//Ricardo Alvarado
//Universidad de Costa Rica
//Junio-julio 2010
namespace SimSistemaColasSinPrioridadAdquirida
{
    /// <summary>
    /// Es el cerebro del modelo. Se encarga de controlar el ingreso de clientes, manejo de colas, salida de clientes.  Llama al estadisticador cuando sucede un nuevo evento, y se encarga de desplegar las estadísticas del estadisticador en pantalla una vez terminada la simulación.
    /// </summary>
    public class Simulador
    {
        public int valorInfinito=9999999;
        public Double[] lambda;//λ=llegadas por minuto, 1/λ=tiempoe entre llegadas
        public Double[] transcursoCliente;
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
       // public List<Cliente>[] clientesEnColaEnSistema;
        public List<Cliente> historialClientesTipo0;
        public List<Cliente> historialClientesTipo1;
        public List<Cliente> historialClientesTipo2;
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
        /// Cantidad de clientes esperados en la cola, de cada tipo
        /// </summary>
        public Double[] Lq;
        public Double[] Wq;
        //variables malas
        public int clientesColas;

        Reportador reportador;
        public int contadorEventos;
        Queue q = new Queue();
        Queue colaClientes = new Queue();
        public List<Cliente> historialClientesTipo;
        public Random r;

        public Estadisticador estadisticador;

       //Double lambdaBuena;



        public Simulador(Double lambda0,Double lambda1,Double lambda2,Double mu,int MX)
        {
            r = new Random();
            inicializarVariables(lambda0,lambda1,lambda2,mu);
           // historialClientes = new List<Cliente>();
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
        public void inicializarVariables(Double ly0, Double ly1, Double ly2, Double mu)
        {
            lambda = new Double[3];
            lambda[0] = ly0;
            lambda[1] = ly1;
            lambda[2] = ly2;
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

  
            Lq = new Double[3];
            Lq[0] = 0;
            Lq[1] = 0;
            Lq[2] = 0;
            Wq = new Double[3];
            Wq[0] = 0;
            Wq[1] = 0;
            Wq[2] = 0;

            historialClientesTipo0 = new List<Cliente>();
            historialClientesTipo1 = new List<Cliente>();
            historialClientesTipo2 = new List<Cliente>();
            transcursoCliente = new Double[3];
            transcursoCliente[0] = ly0;
            transcursoCliente[1] = ly1;
            transcursoCliente[2] = ly2;
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
                Lq[0] = Lq[0] + estadisticador.dominioMiliSegundo[i].Lq[0];
                Lq[1] = Lq[1] + estadisticador.dominioMiliSegundo[i].Lq[1];
                Lq[2] = Lq[2] + estadisticador.dominioMiliSegundo[i].Lq[2];
            }
            Lq[0] = Lq[0] / estadisticador.dominioMiliSegundo.Count;
            Lq[1] = Lq[1] / estadisticador.dominioMiliSegundo.Count;
            Lq[2] = Lq[2] / estadisticador.dominioMiliSegundo.Count;
            Lq[0] = Lq[0] / 100;// me devuevo a minutos, porque el muestreo fue hecho en milisegundos
            Lq[1] = Lq[1] / 100;// me devuevo a minutos, porque el muestreo fue hecho en milisegundos
            Lq[1] = Lq[2] / 100;// me devuevo a minutos, porque el muestreo fue hecho en milisegundos


            for (int i = 0; i < historialClientesTipo0.Count; i++ )
            {
                Wq[0] = Wq[0] + ((Cliente)historialClientesTipo0[i]).tiempoEsperaCola;
                
            }

            Wq[0] = Lq[0] / transcursoCliente[0];

            for (int i = 0; i < historialClientesTipo1.Count; i++)
            {
                Wq[1] = Wq[1] + ((Cliente)historialClientesTipo1[i]).tiempoEsperaCola;
            }

            Wq[1] = Lq[1] / transcursoCliente[1];

            for (int i = 0; i < historialClientesTipo2.Count; i++)
            {
                Wq[2] = Wq[2] + ((Cliente)historialClientesTipo2[i]).tiempoEsperaCola;
            }

            Wq[2] = Lq[2] / transcursoCliente[2];
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
                    clienteASalir.setTMFinal(TM);

                    if (clienteASalir.tipo==0)
                    {
                        historialClientesTipo0.Add(clienteASalir);
                    }
                    if (clienteASalir.tipo == 1)
                    {
                        historialClientesTipo1.Add(clienteASalir);
                    }
                    if (clienteASalir.tipo == 2)
                    {
                        historialClientesTipo2.Add(clienteASalir);
                    }


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
    
        }
        private Double generarST(){
           
            Double Ws = 1 / mu;//Ws=tiempo promedio de un cliente en el servicio
            return Ws;
        }

        /// <summary>
        /// Generars Tiempo entre llegadas según tipo cliente
        /// </summary>
        /// <param name="tipoCliente">The tipo cliente.</param>
        /// <returns></returns>
        private Double generarIT(int tipoCliente)//           1/λ
        {
            Double valor = 0;
    
            Double promedio=0;
            for (int i = 0; i <100; i++)
            {
  
                Double rand = r.NextDouble();
           
                if (rand == 0)
                {
                 
                    rand = 0.00000001;
                }
                valor = (Math.Log(rand) / (lambda[tipoCliente] * -1));
                promedio += valor;
            }
            promedio = promedio / 100;

            return promedio;
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
