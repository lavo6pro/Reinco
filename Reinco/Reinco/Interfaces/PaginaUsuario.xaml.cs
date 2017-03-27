﻿using Reinco.Gestores;
using Reinco.Interfaces.Supervision;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Reinco.Interfaces
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PaginaUsuario : ContentPage
    {
        public ObservableCollection<SupervisionItem> supervisionItem { get; set; } // Medo de colecion observable para listar Las Obras A Supervisar
        public PaginaUsuario()
        {
            InitializeComponent(); // inicializa todo los componentes de la UI

            if (Application.Current.Properties.ContainsKey("cargoUsuario")) // condisional que busca si cargo usuario exite este valo fue almacenado el iniciar sesión
            {
                string cargo = Application.Current.Properties["cargoUsuario"].ToString(); // Recuperando el cargo string y alamacenando en una variable cargo

                #region ++++++++++++++++++++++++++++++++++++++ General ++++++++++++++++++++++++++++++++++++++
                // --------------- Imprimiendo datos del usuario logeado --------------- //
                    if (Application.Current.Properties.ContainsKey("nombreUsuario"))
                    {
                        nombreUsuario.Text = Application.Current.Properties["nombreUsuario"].ToString();
                    }
                    if (Application.Current.Properties.ContainsKey("apellidoUsuario"))
                    {
                        apellidoUsuario.Text = Application.Current.Properties["apellidoUsuario"].ToString();
                    }
                    cargoUsuario.Text = cargo;
                #endregion


                #region //============================ Zona Administrador ===================================//
                    if (cargo == "Administrador")
                    {
                        supervisarListView.IsEnabled = false;
                        supervisarListView.IsVisible = false;
                        interfazResponsable.IsVisible = false;
                        }
                #endregion



                #region ++ =============================== Zona Responsable =============================== ++
                    if (cargo == "Responsable")
                    {
                        interfazAdministrador.IsVisible = false;
                        supervisarListView.IsEnabled = false;
                        supervisarListView.IsVisible = false;
                    }

                #endregion



                #region ** =============================== Zona Supervision =============================== **
                    if (cargo == "Supervision")
                    {
                        interfazAdministrador.IsVisible = false;
                        interfazResponsable.IsVisible = false;
                        supervisionItem = new ObservableCollection<SupervisionItem>();
                        // +-----+ Cargando Obras A supervisar +-----+
                        CargarSupervisionItem();
                        // +-----+ Listando la obras a supervisar  +-----+
                        supervisarListView.ItemsSource = supervisionItem;
                    }
                #endregion


            }
        }


        #region // =============================== Iniciar Supervision =============================== //
        public void supervisar(object sender, EventArgs e)
            {
                var idupervisar = ((MenuItem)sender).CommandParameter.ToString();
                DisplayAlert("Supervisar", idupervisar + " more context action", "OK");
                Navigation.PushAsync(new PaginaSupervision());
            } 
        #endregion



        #region // =============================== Cargando Datos Con Iteracion Para Supervision =============================== //
        private void CargarSupervisionItem()
            {
                for (int i = 0; i < 50; i++)
                {
                    supervisionItem.Add(new SupervisionItem
                    {
                        titulo = "Titulo De La Tarea",
                        descripcion = "Descripcion De La Tarea",
                        numeroTarea = Convert.ToString(i),
                        id = Convert.ToString(i),
                    });
                }
            }
        #endregion


    }
}
