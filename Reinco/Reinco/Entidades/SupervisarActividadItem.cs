﻿using Newtonsoft.Json;
using Plugin.Media;
using Reinco.Recursos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Reinco.Entidades
{
    public class SupervisarActividadItem : INotifyPropertyChanged
    {
        WebService Servicio = new WebService();
        string Mensaje;
        VentanaMensaje mensaje = new VentanaMensaje();
        public int idSupervisionActividad { get; set; }
        public string item { get; set; }
        public string actividad { get; set; }
        public bool aprobacion { get; set; }
        public bool observacionLevantada { get; set; }
        public string anotacionAdicinal { get; set; }
        public string tolerancia { get; set; }



        public ICommand guardarItem { get; private set; }


        public SupervisarActividadItem()
        {

            guardarItem = new Command(() =>
            {
                // App.Current.MainPage.DisplayAlert("Aceptar", this.actividad + this.anotacionAdicinal, "Aceptar");
                GuardarActividad();
            });


            AprobacionCommand = new Command(() =>
            {
                App.Current.MainPage.DisplayAlert("Me Ejecute", "OK", "Aceptar");
                if (aprobacion == true)
                {
                    App.Current.MainPage.DisplayAlert("Me Ejecute", ObsLevActivar + " En Falso", "Aceptar");
                    ObsLevActivar = false;
                }
                else
                {
                    App.Current.MainPage.DisplayAlert("Me Ejecute", ObsLevActivar + " En Verdadesro", "Aceptar");
                    ObsLevActivar = true;
                } 
            });

            #region ================= Expandir =================
            MostrarAnotacion = false;
            ExpandirAnotacion = new Command(() =>
            {
                if (toggle == 0)
                {
                    MostrarAnotacion = true;
                    toggle = 1;
                }
                else
                {
                    MostrarAnotacion = false;
                    toggle = 0;
                }
            }); 
            #endregion



            #region ================ Uso De La Camara ================
            EncenderCamara = new Command(async () =>
                {
                    await CrossMedia.Current.Initialize(); // Inicializando la libreri

                // Verificando si el dispotivo tiene Camara
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                    {
                        await App.Current.MainPage.DisplayAlert("Error", ":( No hay cámara disponible.", "Aceptar");
                    }

                // Directorio para almacenar la imagen
                var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    Directory = "fotos",
                    Name = "fotoreinco.jpg",
                    AllowCropping = true,
                });

                // mostrando la imagen en la interfas del telefono
                if (file != null)
                {
                    await App.Current.MainPage.DisplayAlert("Localizacion Del Archivo", file.Path, "OK");

                    RutaImagen = ImageSource.FromStream(() =>
                    {
                        var stream = file.GetStream();
                        file.Dispose();
                        return stream;
                    });
                    

                    // Preparando la foto para enviar al webservice
                    var foto = file.GetStream();
                    var fotoArray = ReadFully(foto);


                    // Retratado 
                    var streamTratado = new MemoryStream(fotoArray);
                    this.ImagenTratado = streamTratado;


                }
                // End Camera
            });
            #endregion


        }

        public ICommand AprobacionCommand { get; private set; }
        public bool obsLevActivar { get; set; }
        public bool ObsLevActivar
        {
            set
            {
                if (obsLevActivar != value)
                {
                    obsLevActivar = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ObsLevActivar"));
                }
            }
            get
            {
                return obsLevActivar;
            }
        }

        #region ================ Expadir Actividad Por Cada Items Correspondiente ================
        int toggle = 0;
        public ICommand ExpandirAnotacion { get; private set; }
        public bool mostrarAnotacion { get; set; }
        public bool MostrarAnotacion
        {
            set
            {
                if (mostrarAnotacion != value)
                {
                    mostrarAnotacion = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MostrarAnotacion"));
                }
            }
            get
            {
                return mostrarAnotacion;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion


        #region ================= Preparando la Interaccion De la Camara =================
        // Preparadndo la imagen para enviar


        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }


        public byte [] fotoArray { get; set; } // Array De Bits Para Enviar la Foto Al Web Service

        private ImageSource imagenTratado { get; set; }
        public ImageSource ImagenTratado
        {
            set
            {
                if (imagenTratado != value)
                {
                    imagenTratado = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ImagenTratado"));
                }
            }
            get
            {
                return imagenTratado;
            }
        }
        // InterAccion Con la Camara
        private ImageSource rutaImagen;
        public ImageSource RutaImagen
        {
            set
            {
                if (rutaImagen != value)
                {
                    rutaImagen = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RutaImagen"));
                }
            }
            get
            {
                return rutaImagen;
            }
        }
        public ICommand EncenderCamara { get; private set; }
        #endregion

        public async void GuardarActividad() {
            try
            {
                int Si, No;
                if (aprobacion == false) {
                    Si = 0;
                    No = 1;
                }
                else {
                    Si = 1;
                    No = 0;
                }
                    object[,] variables = new object[,] {
                        { "idSupervisionActividad",idSupervisionActividad  } ,{ "si", Si },{ "no", No },
                        { "observacionLevantada", No }, { "anotacionAdicional", anotacionAdicinal }};

                dynamic result = await Servicio.MetodoGetString("SupervisionActividad.asmx", "guardarSupervisionActividad", variables);
                    Mensaje = Convert.ToString(result);
                    if (result != null)
                    {
                        await App.Current.MainPage.DisplayAlert("Guardar actividad", Mensaje, "OK");
                        return;
                    }
            }
            catch (Exception ex)
            {
                await mensaje.MostrarMensaje("Agregar Usuario", "Error en el dispositivo o URL incorrecto: " + ex.ToString());
            }
        }

    }
}
