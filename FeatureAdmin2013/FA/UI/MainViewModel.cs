﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FA.Models;
using FA.SharePoint;
using FA.UI.Features;
using FA.UI.Locations;


namespace FA.UI
{
    public class MainViewModel : FA.UI.BaseClasses.ViewModelBase
    {
        #region Fields
        private IFeaturesListViewModel _featuresListViewModel;
        private ILocationsListViewModel _locationsListViewModel;


        private BackgroundWorker backgroundWorker;
        private int iterations = 50;
        private int progressPercentage = 0;
        private string status;
        private bool loadingBusy = false;

        //private IFeatureViewModel _selectedFeatureDefinition;

        //private ILocationViewModel _selectedLocation;



        #endregion

        #region Bindable Properties

        public ObservableCollection<FeatureDefinition> FeatureDefinitions;
        public ObservableCollection<FeatureParent> Parents;
        public int Iterations
        {
            get { return iterations; }
            set
            {
                if (iterations != value)
                {
                    iterations = value;
                    OnPropertyChanged("Iterations");
                }
            }
        }

        public int ProgressPercentage
        {
            get { return progressPercentage; }
            set
            {
                if (progressPercentage != value)
                {
                    progressPercentage = value;
                    OnPropertyChanged("ProgressPercentage");
                }
            }
        }

        public string Status
        {
            get { return status; }
            set
            {
                if (status != value)
                {
                    status = value;
                    OnPropertyChanged("Status");
                }
            }
        }

        public bool LoadingBusy
        {
            get { return loadingBusy; }
            set
            {
                if (loadingBusy != value)
                {
                    loadingBusy = value;
                    OnPropertyChanged("LoadEnabled");
                }
            }
        }

        #endregion Bindable Properties

        public MainViewModel(
            IFeaturesListViewModel featuresListViewModel,
            ILocationsListViewModel locationsListViewModel
            )
        {
            _featuresListViewModel = featuresListViewModel;
            _locationsListViewModel = locationsListViewModel;

            backgroundWorker = new BackgroundWorker();
            // Background Process
            backgroundWorker.DoWork += backgroundWorker_DoWorkGetFeatureDefinitions;
            backgroundWorker.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;

            // Progress
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.ProgressChanged += backgroundWorker_ProgressChanged;

        }
        
        public void Load()
        {
            LoadingBusy = true;
            Status = "Loading SharePoint Features ...";

            _featuresListViewModel.Load();

            

            LoadingBusy = false;
        }

        #region BackgroundWorker Events

        // Runs on Background Thread
        private void backgroundWorker_DoWorkGetFeatureDefinitions(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            // int result = 0;

            //foreach (var current in processor)
            //{
            //    if (worker != null)
            //    {

            //        if (worker.WorkerReportsProgress)
            //        {
            //            int percentageComplete =
            //                (int)((float)current / (float)iterations * 100);
            //            string progressMessage =
            //                string.Format("Iteration {0} of {1}", current, iterations);
            //            worker.ReportProgress(percentageComplete, progressMessage);
            //        }
            //    }
            //    result = current;
            //}

            var spDefs = FarmRead.GetFeatureDefinitionCollection();

            var result = new ObservableCollection<FeatureDefinition>(FeatureDefinition.GetFeatureDefinition(spDefs));

            e.Result = result;
        }

        // Runs on UI Thread
        private void backgroundWorker_RunWorkerCompleted(object sender,
            RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Status  = e.Error.Message;
            }
            else if (e.Cancelled)
            {
                Status = "Cancelled";
            }
            else
            {
                FeatureDefinitions = e.Result as ObservableCollection<FeatureDefinition>;
                Status = FeatureDefinitions[0].Name;
                ProgressPercentage = 0;
            }
            LoadingBusy = !backgroundWorker.IsBusy;

        }

        // Runs on UI Thread
        private void backgroundWorker_ProgressChanged(object sender,
            ProgressChangedEventArgs e)
        {
            ProgressPercentage = e.ProgressPercentage;
            Status = (string)e.UserState;
        }

        #endregion
    }
}