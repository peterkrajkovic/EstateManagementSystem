//KDTree<int, double> tree = new KDTree<int, double>(2);

//var addedPoints = Generator.RandomTreeOperation(ref tree, 2,50);

//var rangedResults = tree.RangeFind(new int[] { 1, 1 }, new int[] { 1000, 1000 });
//Console.WriteLine();



// Usage
using Backend;
using Classes;
using Classes.structures;
using static Backend.Generator;

//var tree = GenerateRandomLambdaTree(0,0);
//var added = RandomLambdaTreeOperation(ref tree, 1000);

Core c = new Core();
c.InsertParcel("1P", 0, 0, 0, 10, 10);
c.InsertProperty("1Prop",0,2,2,3,3);

var items = c.RangeFindProperties(2.5,2.5);
if (items.Count() > 0)
{
    foreach (Property item in items)
    {
        for (int i = 0; i < item.References.Count(); i++)
        {
            item.References[i].Description = "papa";
        }
    }
}

var x = c.RangeFindParcels(2,2);
Console.WriteLine();


