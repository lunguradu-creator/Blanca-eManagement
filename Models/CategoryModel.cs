
public class CategoryModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public int? ParentCategoryId { get; set; }
    public virtual CategoryModel? ParentCategory { get; set; }  // Nullable - problema
    public virtual ICollection<ProductModel> Products { get; set; } = new List<ProductModel>();

    public static implicit operator CategoryModel(string v)
    {
        throw new NotImplementedException();
    }
}

