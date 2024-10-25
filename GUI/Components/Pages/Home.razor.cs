
using Backend;
using Classes;
using Classes.structures;
using GUI.Components.Client;
using GUI.Components.Individual;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace GUI.Components.Pages
{
    public partial class Home
    {
      
        private List<Estate>? findResults {  get; set; }
        private IList<IBrowserFile> _files = new List<IBrowserFile>();
        private int PaperView { get; set; } = 0;

        private void LoadRandomTrees()
        {
            ClientClass.LoadRandomTrees(1000,1000,10);
            VisualizeAll();
        }
        private void VisualizeAll()
        {
            Model.VisualizationNodeAll = ClientClass.VisualizeAll();
            Model.VisualizationNodeParcels = ClientClass.VisualizeParcels();
            Model.VisualizationNodeProperties = ClientClass.VisualizeProperties();
            StateHasChanged();
        }
       
        private void UploadFiles(IBrowserFile file)
        {
            _files.Add(file);
            //TODO upload the files to the server
        }
        private void SaveFiles()
        {

        }
        private void DeleteTrees()
        {
            ClientClass.DeleteTrees();
            VisualizeAll();
        }

        protected override async Task OnAfterRenderAsync(bool isFirst)
        {
            if (isFirst)
            {
                VisualizeAll();
            }
            StateHasChanged();
        }



        private async void Find()
        {
            findResults = null;
            Parcel newParcel = new Parcel();
            var options = new DialogOptions { CloseOnEscapeKey = true };
            FindDialog.FindModel findModel = new FindDialog.FindModel();
            var parameters = new DialogParameters<FindDialog>
            {
                { x => x.Model, findModel }
            };

            var result = await DialogService.Show<FindDialog>("Estate Find", parameters, options).Result;
            if (result != null && !result.Canceled && result.Data != null && result.Data is FindDialog.FindModel)
            {
                FindDialog.FindModel model = (FindDialog.FindModel)result.Data;
                switch (model.EstateType)
                {
                    case 0:
                        findResults = ClientClass.FindProperties(model.X, model.Y);
                        break;
                    case 1:
                        findResults = ClientClass.FindParcels(model.X, model.Y);
                        break;
                    default:
                        findResults = ClientClass.FindAll(model.X, model.Y);
                        break;
                }
                if (findResults == null )
                {
                    Snackbar.Add("Tree has no estates. Add some first.", Severity.Normal);
                } 
                else if (findResults.Count() == 0)
                {
                    Snackbar.Add("No estates are overlapping given position.", Severity.Normal);
                }
            }
        }

        private async void InsertNew()
        {
            Parcel newParcel = new Parcel();
            var options = new DialogOptions { CloseOnEscapeKey = true };

            var estateModel = new InsertEstateDialog.EstateModel();
            var parameters = new DialogParameters<InsertEstateDialog>
            {
                { nameof(InsertEstateDialog.Model), estateModel }
            };

            var result = await DialogService.Show<InsertEstateDialog>("Insert Estate", parameters, options).Result;
            if (result != null && !result.Canceled && result.Data != null && result.Data is InsertEstateDialog.EstateModel)
            {
                InsertEstateDialog.EstateModel model = (InsertEstateDialog.EstateModel)result.Data;
                switch (model.EstateType)
                {
                    case 0:
                        ClientClass.InsertProperty(model.Description, model.Number, model.X1, model.Y1, model.X2, model.Y2);
                        break;
                    default:
                        ClientClass.InsertParcel(model.Description, model.Number, model.X1, model.Y1, model.X2, model.Y2);
                        break;
                }
            }
            VisualizeAll();
        }
    }
}
