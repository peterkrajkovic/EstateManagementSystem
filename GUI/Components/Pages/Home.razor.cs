
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
        public static bool IsVizualize = true;
        private List<Estate>? findResults { get; set; }
        private int PaperView { get; set; } = 0;
        private FindDialog.FindModel? LastFindModel { get; set; }

        private async void LoadRandomTrees()
        {
            var options = new DialogOptions { CloseOnEscapeKey = true };
            var parameters = new DialogParameters<LoadRandomTreesDialog>();

            var result = await DialogService.Show<LoadRandomTreesDialog>("Load Random Trees", options).Result;
            if (result != null && !result.Canceled && result.Data != null && result.Data is LoadRandomTreesDialog.LoadRandomTreesDialogData)
            {
                var dialogData = (LoadRandomTreesDialog.LoadRandomTreesDialogData)result.Data;
                Controller.LoadRandomTrees((int)dialogData.ParcelCount, (int)dialogData.PropertyCount, (double)dialogData.Coverage);
                findResults = null;
                VisualizeAll();
                Snackbar.Add("Estates loaded", Severity.Success);
            }
            
        }
        private void VisualizeAll()
        {
            if (IsVizualize)
            {
            Model.VisualizationNodeAll = Controller.VisualizeAll();
            Model.VisualizationNodeParcels = Controller.VisualizeParcels();
            Model.VisualizationNodeProperties = Controller.VisualizeProperties();
            StateHasChanged();
            }
        }

        private void EstateChanged()
        {
            if (LastFindModel != null)
            {
                FindByModel(LastFindModel);
            }
            Snackbar.Add("Estate updated successfully.", Severity.Success);
            VisualizeAll();
        }
        private void EstateDeleted()
        {
            if (LastFindModel != null)
            {
                FindByModel(LastFindModel);
            }
            Snackbar.Add("Estate deleted successfully.", Severity.Success);
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
                    Snackbar.Add("Parsing file.",Severity.Normal);
                    if (Controller.LoadFile(content))
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
            string fileText = Controller.SaveFile();
            byte[] byteArray = Encoding.UTF8.GetBytes(fileText);

            var fileStream = new MemoryStream(byteArray);
            using var streamRef = new DotNetStreamReference(stream: fileStream);

            await JS.InvokeVoidAsync("downloadFileFromStream", FILENAME, streamRef);
        }

        private void DeleteTrees()
        {
            Controller.DeleteTrees();
            LastFindModel = null;
            Snackbar.Add("Trees deleted", Severity.Success);
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

                #region Change directions by chars
                if (model.X1Char != null && model.X1Char == 'W')
                {
                    model.X1 = model.X1 * -1;
                }
                if (model.X2Char != null && model.X2Char == 'W')
                {
                    model.X2 = model.X2 * -1;
                }
                if (model.Y1Char != null && model.Y1Char == 'S')
                {
                    model.Y1 = model.Y1 * -1;
                }
                if (model.Y2Char != null && model.Y2Char == 'S')
                {
                    model.Y2 = model.Y2 * -1;
                }
                #endregion

                //save model for reloading after Estate update or delete
                LastFindModel = model;

                FindByModel(model); //backend call
                
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
        private async void FindByModel(FindDialog.FindModel model)
        {
            switch (model.EstateType)
            {
                case 0:
                    if (model.GPS == 0)
                    {
                        findResults = Controller.FindProperties((double)model.X1, (double)model.Y1);
                    }
                    else
                    {
                        findResults = Controller.RangeFindProperties((double)model.X1, (double)model.Y1, (double)model.X2, (double)model.Y2);
                    }
                    break;
                case 1:
                    if ((double)model.GPS == 0)
                    {
                        findResults = Controller.RangeFindParcels((double)model.X1, (double)model.Y1);
                    }
                    else
                    {
                        findResults = Controller.RangeFindParcels((double)model.X1, (double)model.Y1, (double)model.X2, (double)model.Y2);
                    }
                    break;
                default:
                    if (model.GPS == 0)
                    {
                        findResults = Controller.RangeFindAll((double)model.X1, (double)model.Y1);
                    }
                    else
                    {
                        findResults = Controller.RangeFindAll((double)model.X1, (double)model.Y1, (double)model.X2, (double)model.Y2);
                    }
                    break;
            }
            if (model.Boundaries == 0) // filter points that share boundaries
            {
                if (model.GPS == 1) //2 point search
                {
                    findResults = findResults.Where(x => 
                        (x.LeftBottom.Width.CompareTo((double)model.X2) == 0 && x.LeftBottom.Height.CompareTo((double)model.Y2) == 0) 
                    ||  (x.LeftBottom.Width.CompareTo((double)model.X1) == 0 && x.LeftBottom.Height.CompareTo((double)model.Y1) == 0) 
                    || (x.RightTop.Width.CompareTo((double)model.X2) == 0 && x.RightTop.Height.CompareTo((double)model.Y2) == 0)
                    || (x.RightTop.Width.CompareTo((double)model.X1) == 0 && x.RightTop.Height.CompareTo((double)model.Y1) == 0)
                    ).ToList();
                } else //1 point search
                {
                    findResults = findResults.Where(x =>
                 (x.LeftBottom.Width.CompareTo((double)model.X1) == 0 && x.LeftBottom.Height.CompareTo((double)model.Y1) == 0)
                 || (x.RightTop.Width.CompareTo((double)model.X1) == 0 && x.RightTop.Height.CompareTo((double)model.Y1) == 0)
                 ).ToList();
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
                        Controller.InsertProperty(model.Description, model.Number, model.X1, model.Y1, model.X2, model.Y2);
                        break;
                    default:
                        Controller.InsertParcel(model.Description, model.Number, model.X1, model.Y1, model.X2, model.Y2);
                        break;
                }
            }
            VisualizeAll();
        }
    }
}
