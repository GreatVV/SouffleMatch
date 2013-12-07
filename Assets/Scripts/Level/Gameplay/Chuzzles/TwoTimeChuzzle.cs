public class TwoTimeChuzzle : Chuzzle
{   
    public override void Destroy(bool needCreateNew, bool withAnimation = true)
    {
        NeedCreateNew = false;
        TilesFactory.Instance.CreateChuzzle(Current, Color);
        Die(false);           
    }

    protected override void OnAwake()
    {
        
    }
}