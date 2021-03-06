﻿using DevExpress.Mvvm.DataAnnotations;
using SmartLawyer.Models;
using SmartLawyer.Models.Classes;
using SmartLawyer.Models.Values;
using SmartLawyer.Utils;
using SmartLawyer.ViewModels.Main;
using SmartLawyer.ViewModels.PersonsVMs;
using SmartLawyer.Views.Controls.Persons;
using SmartLawyer.Views.Controls.Users;
using SmartLawyer.Views.Person;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace SmartLawyer.ViewModels.PersonVMs
{
    public class VMPersons : MarkupExtension, VMManagmentSystem<PersonsModel>
    {

        public VMPersons()
        {
            AdvancedSearchContent = new UCPersonAdvancedSearch
            {
                DataContext = this
            };
        }





        public virtual string Title { get; set; } = "PersonsTitle".GetDictionaryValue();
        public virtual ImageSource ImageTitle { get; set; } = "personstitle".ToImageSource();
        public virtual string SearchKey { get; set; }
        public virtual object AdvancedSearchContent { get; set; }
        public ObservableCollection<PersonsModel> DataGridSource { get; set; }
            = new ObservableCollection<PersonsModel>();// = DataAccess.PersonsData();
        [BindableProperty(OnPropertyChangedMethodName = nameof(SelectedConstantChanged), OnPropertyChangingMethodName = nameof(SelectedConstantChanging))]
        public virtual PersonsModel SelectedDataItem { get; set; }
        public virtual bool ShowAdvancedSearch { get; set; }
        public virtual double RotateAngle { get; set; }
        public virtual bool DeletePopup { get; set; }
        public virtual Brush ViewModelButtonColor { get; set; } = (Brush)(new BrushConverter().ConvertFromString(SystemValues.MyColors.Default));
        public virtual object MainContentValue { get; set; } = new UCPersonsMain();
        public virtual bool IsInProgress { get; set; } = false;
        [BindableProperty(OnPropertyChangedMethodName = nameof(CheckAllChanged), OnPropertyChangingMethodName = nameof(CheckAllChanging))]
        public virtual bool IsCheckAll { get; set; }

        List<PersonsModel> Persons = new List<PersonsModel>();
        List<PersonsAddressModel> PersonsAddress = new List<PersonsAddressModel>();
        List<PersonsCommunicationModel> PersonsCommunication = new List<PersonsCommunicationModel>();
        List<CodesModel> PersonsTypes = new List<CodesModel>();
        List<CodesModel> CommTypes = new List<CodesModel>();

        public void Add()
        {
            VPersonAdd add = new VPersonAdd(PersonsTypes, CommTypes);
            if (add.ShowDialog() == true)
            {
                var dataContext = (add.DataContext as VMPersonAdd);
                PersonsCommunication.AddRange(dataContext.AddedCommunication);
                PersonsAddress.AddRange(dataContext.AddedAddress);
                var AddedPerson = dataContext.AddedPerson;
                var adress = PersonsAddress.Where(x => x.PeAdPerIdFk == AddedPerson.PeId).FirstOrDefault();
                if (adress != null)
                    AddedPerson.PeAddress = $"{adress.PeAdCity.Trim()} - {adress.PeAdStreetName.Trim()}";
                AddedPerson.Type = PersonsTypes.Where(x => x.CId == AddedPerson.PeType).FirstOrDefault().CName;
                var communication = PersonsCommunication.Where(x => x.CoPeIdFk == AddedPerson.PeId);
                AddedPerson.PhoneNo = communication.Where(x => x.CoNameCfk.Equals(SystemValues.Communications.Phone)).FirstOrDefault()?.CoValue;
                AddedPerson.MobileNo = communication.Where(x => x.CoNameCfk.Equals(SystemValues.Communications.Mobile)).FirstOrDefault()?.CoValue;
                AddedPerson.Email = communication.Where(x => x.CoNameCfk.Equals(SystemValues.Communications.Emial)).FirstOrDefault()?.CoValue;

                Persons.Add(AddedPerson);
                DataGridSource.Add(dataContext.AddedPerson);
            }
        }
        public void AdvancedSearchTogel()
        {
            ShowAdvancedSearch = !ShowAdvancedSearch;
        }
        public void Archive()
        {

        }

        protected void SelectedConstantChanged(PersonsModel oldValue)
        {

        }
        protected void SelectedConstantChanging(PersonsModel newValue)
        {

        }

        protected void CheckAllChanged(bool oldValue)
        {

        }
        protected void CheckAllChanging(bool newValue)
        {
            foreach (var item in DataGridSource)
            {
                item.IsChecked = newValue;
            }
        }

        public void Delete()
        {
            if (SelectedDataItem != null)
            {
                if (MessageBox.Show("Are you sure you want to delete all selected Persons??",
                "Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    foreach (var item in DataGridSource)
                    {
                        if (item.IsChecked)
                        {
                            DataAccess.DeletePerson(item.PeId);
                            DataAccess.DeletePersonCommunication(item.PeId);
                            DataAccess.DeletePersonAddress(item.PeId);
                        }
                    }
                    DataGridSource.ReFill(DataGridSource.Where(x => !x.IsChecked).ToList());
                }
            }
        }

        public void DoAdvancedSearch()
        {

        }

        public void Edit()
        {
            var person = new PersonsModel()
            {
                PeId = SelectedDataItem.PeId,
                PeAddress = SelectedDataItem.PeAddress,
                IsChecked = SelectedDataItem.IsChecked,
                PeIdentity = SelectedDataItem.PeIdentity,
                PeName = SelectedDataItem.PeName,
                PeType = SelectedDataItem.PeType,
            };
            VPersonEdit edit = new VPersonEdit(person, PersonsTypes, CommTypes, PersonsCommunication.Where(x => x.CoPeIdFk == person.PeId).ToList());
            if (edit.ShowDialog() == true)
            {
                var dataContext = edit.DataContext as VMPersonEdit;

                Persons.Remove(SelectedDataItem);
                DataGridSource.Remove(SelectedDataItem);
                PersonsCommunication.RemoveAll(x => x.CoPeIdFk == dataContext.EditedPerson.PeId);
                PersonsCommunication.AddRange(dataContext.AddedCommunication);
                PersonsAddress.RemoveAll(x => x.PeAdPerIdFk == dataContext.EditedPerson.PeId);
                PersonsAddress.AddRange(dataContext.AddedAddress);

                var adress = PersonsAddress.Where(x => x.PeAdPerIdFk == dataContext.EditedPerson.PeId).FirstOrDefault();
                if (adress != null)
                    dataContext.EditedPerson.PeAddress = $"{adress.PeAdCity} - {adress.PeAdStreetName}";
                dataContext.EditedPerson.Type = PersonsTypes.Where(x => x.CId == dataContext.EditedPerson.PeType).FirstOrDefault().CName;
                var communication = PersonsCommunication.Where(x => x.CoPeIdFk == dataContext.EditedPerson.PeId);
                dataContext.EditedPerson.PhoneNo = communication.Where(x => x.CoNameCfk.Equals(SystemValues.Communications.Phone)).FirstOrDefault()?.CoValue;
                dataContext.EditedPerson.MobileNo = communication.Where(x => x.CoNameCfk.Equals(SystemValues.Communications.Mobile)).FirstOrDefault()?.CoValue;
                dataContext.EditedPerson.Email = communication.Where(x => x.CoNameCfk.Equals(SystemValues.Communications.Emial)).FirstOrDefault()?.CoValue;

                Persons.Add(dataContext.EditedPerson);
                DataGridSource.Add(dataContext.EditedPerson);
                SelectedDataItem = dataContext.EditedPerson;
            }

        }
        public void Export()
        {

        }

        public override object ProvideValue(IServiceProvider serviceProvider)
            => this;
        public void Refresh()
        {
            new Thread(() =>
            {
                Thread inProgress = new Thread(() =>
                {
                    IsInProgress = true;
                    try
                    {
                        Persons = DataAccess.PersonsData();
                        PersonsAddress = DataAccess.PersonsAddressData();
                        PersonsCommunication = DataAccess.PersonsCommunicationData();
                        PersonsTypes = DataAccess.CodesData(SystemValues.MasterSystemConstants.PersonType);
                        CommTypes = DataAccess.CodesData(SystemValues.MasterSystemConstants.CommunicationType);
                    }
                    catch { MessageBox.Show("could not open connection with server!\nCheck your internet connection or server is connected", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning); }
                    foreach (var item in Persons)
                    {
                        var adress = PersonsAddress.Where(x => x.PeAdPerIdFk == item.PeId).FirstOrDefault();
                        if (adress != null)
                            item.PeAddress = $"{adress.PeAdCity.Trim()} - {adress.PeAdStreetName.Trim()}";
                        var communication = PersonsCommunication.Where(x => x.CoPeIdFk == item.PeId);
                        item.Type = PersonsTypes.Where(x => x.CId == item.PeType).FirstOrDefault()?.CName;
                        item.PhoneNo = communication.Where(x => x.CoNameCfk.Equals(SystemValues.Communications.Phone)).FirstOrDefault()?.CoValue;
                        item.MobileNo = communication.Where(x => x.CoNameCfk.Equals(SystemValues.Communications.Mobile)).FirstOrDefault()?.CoValue;
                        item.Email = communication.Where(x => x.CoNameCfk.Equals(SystemValues.Communications.Emial)).FirstOrDefault()?.CoValue;
                    }
                })
                { IsBackground = true };
                inProgress.Start();
                while (inProgress.IsAlive)
                {
                    if (RotateAngle > 360)
                        RotateAngle = 0;
                    RotateAngle += 18.25;
                    Thread.Sleep(50);
                }
                while (RotateAngle > 0 && RotateAngle < 365)
                {
                    RotateAngle += 18.25;
                    Thread.Sleep(50);
                }
                RotateAngle = 0;
                DataGridSource.ReFill(Persons);
                IsInProgress = false;
            })
            { IsBackground = true }.Start();
        }
        public void CheckAll()
        {
            foreach (var item in DataGridSource)
            {
                item.IsChecked = true;
            }
        }
        public void UncheckAll()
        {
            foreach (var item in DataGridSource)
            {
                item.IsChecked = false;
            }
        }

        public void View()
        {
            VPersonView view = new VPersonView();
            view.ShowDialog();
        }
    }
}
