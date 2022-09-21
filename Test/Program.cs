using WebControl;
using WebControl.Products;
using WebControl.Products.ValueTypes;

var e = new Endpoint("http://webcontrol.fritz.box");
var rooms = e.GetRooms();
foreach (var r in rooms)
{
    Console.WriteLine("{0}: {1}", r.Id, r.Name);
    foreach (var c in r.Channels)
    {
        Console.WriteLine("\t{0}: {1}", c.Id, c.Name);
        var a = c.Actor;
        if (a is VenetianBlind)
        {
            VenetianBlind blind = a as VenetianBlind;
            Console.WriteLine("\t\tControl: {0}, Product: {1}", blind.Control, blind.ProductType);
            Console.WriteLine("\t\tRunning: {0}, Position: {1:P0}, Angle: {2}º", blind.Running, blind.Position, (int)blind.Angle);

            
            var read = Console.ReadKey();
            if (read.KeyChar == 'u')
            {
                Console.Write("New position: ");
                string? p = Console.ReadLine();
                Console.Write("New angle: ");
                string? angle = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(p)) blind.Position = decimal.Parse(p)/100;
                Thread.Sleep(2000);
                if (!string.IsNullOrWhiteSpace(angle)) blind.Angle = short.Parse(angle);
                Thread.Sleep(2000);
            }
        } else if (c is Scene)
        {
            Console.WriteLine("\t\tScene: {0}", (c.Actor as Scene).SceneIndex);
        }

    }
}