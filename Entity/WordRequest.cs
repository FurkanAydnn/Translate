using Entity.Abstract;
using System.ComponentModel.DataAnnotations;

namespace Entity
{
    public class WordRequest : IEntity
    {
        public int Id { get; set; }
        [Display(Name = "Kelime")]
        public string Word { get; set; }
        [Display(Name = "Dil")]
        public virtual Language Language { get; set; }
        public int LanguageId { get; set; }
        public virtual Person Person { get; set; }
    }
}
