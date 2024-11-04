
using Backend;
using Classes;
using Classes.structures;
using GUI.Components.Client;
using GUI.Components.Individual;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using System.Text;
using static GUI.Components.Client.Model;

namespace GUI.Components.Pages
{
    public partial class Home
    {
        public static readonly string FILENAME = "estates.csv";
        private List<Estate>? findResults { get; set; }
        private int PaperView { get; set; } = 0;

        private async void LoadRandomTrees()
        {
            var options = new DialogOptions { CloseOnEscapeKey = true };
            var parameters = new DialogParameters<LoadRandomTreesDialog>();

            var result = await DialogService.Show<LoadRandomTreesDialog>("Load Random Trees", options).Result;
            if (result != null && !result.Canceled && result.Data != null && result.Data is LoadRandomTreesDialog.LoadRandomTreesDialogData)
            {
                var dialogData = (LoadRandomTreesDialog.LoadRandomTreesDialogData)result.Data;
                ClientClass.LoadRandomTrees((int)dialogData.ParcelCount, (int)dialogData.PropertyCount, (int)dialogData.Coverage);
                findResults = null;
                VisualizeAll();
            }
        }
        private void VisualizeAll()
        {
            Model.VisualizationNodeAll = ClientClass.VisualizeAll();
            Model.VisualizationNodeParcels = ClientClass.VisualizeParcels();
            Model.VisualizationNodeProperties = ClientClass.VisualizeProperties();
            StateHasChanged();
        }
        private void EstateChanged()
        {
            findResults = null;
            Snackbar.Add("Estate updated successfully.", Severity.Success);
            VisualizeAll();
        }

        private async void UploadFiles(IBrowserFile file)
        {
            long maxFileSize = 1024 * 1024 * 100; // 100 MB


            try
            {
                // Open a stream for reading
                using var stream = file.OpenReadStream(maxFileSize);
                using var reader = new StreamReader(stream);

                // Read the stream as a string
                string content = await reader.ReadToEndAsync();
                if (content.Length > 0)
                {
                    if (ClientClass.LoadFile(content))
                    {
                        Snackbar.Add("File loaded correctly.", Severity.Success);
                        VisualizeAll();
                    }
                    else
                    {
                        Snackbar.Add("File loaded incorrectly.", Severity.Error);
                    }
                }
                else
                {
                    Snackbar.Add("File is empty.", Severity.Warning);
                }
            }
            catch (IOException _) //stream could not be opened 
            {
                Snackbar.Add("File not loaded due to exceeded 100 MB limit.", Severity.Error);
            }
        }

        private async void SaveFiles()
        {
            string fileText = ClientClass.SaveFile();
            byte[] byteArray = Encoding.UTF8.GetBytes(fileText);

            var fileStream = new MemoryStream(byteArray);
            using var streamRef = new DotNetStreamReference(stream: fileStream);

            await JS.InvokeVoidAsync("downloadFileFromStream", FILENAME, streamRef);
        }

        private void DeleteTrees()
        {
            ClientClass.DeleteTrees();
            findResults = null;
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
                        if (model.GPS == 0)
                        {
                            findResults = ClientClass.FindProperties((double)model.X1, (double)model.Y1);
                        }
                        else
                        {
                            findResults = ClientClass.RangeFindProperties((double)model.X1, (double)model.Y1, (double)model.X2, (double)model.Y2);
                        }
                        break;
                    case 1:
                        if ((double)model.GPS == 0)
                        {
                            findResults = ClientClass.RangeFindParcels((double)model.X1, (double)model.Y1);
                        }
                        else
                        {
                            findResults = ClientClass.RangeFindParcels((double)model.X1, (double)model.Y1, (double)model.X2, (double)model.Y2);
                        }
                        break;
                    default:
                        if (model.GPS == 0)
                        {
                            findResults = ClientClass.RangeFindAll((double)model.X1, (double)model.Y1);
                        }
                        else
                        {
                            findResults = ClientClass.RangeFindAll((double)model.X1, (double)model.Y1, (double)model.X2, (double)model.Y2);
                        }
                        break;
                }
                if (findResults == null)
                {
                    Snackbar.Add("Tree has no estates. Add some first.", Severity.Normal);
                }
                else if (findResults.Count() == 0)
                {
                    Snackbar.Add("No estates are overlapping given position.", Severity.Normal);
                }
                else
                {
                    PaperView = 1;
                }
            }
        }

        private async void InsertNew()
        {
            Parcel newParcel = new Parcel();
            var options = new DialogOptions { CloseOnEscapeKey = true };

            var estateModel = new EstateModel();
            var parameters = new DialogParameters<InsertEstateDialog>
            {
                { nameof(InsertEstateDialog.Model), estateModel }
            };

            var result = await DialogService.Show<InsertEstateDialog>("Insert Estate", parameters, options).Result;
            if (result != null && !result.Canceled && result.Data != null && result.Data is EstateModel)
            {
                EstateModel model = (EstateModel)result.Data;
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
