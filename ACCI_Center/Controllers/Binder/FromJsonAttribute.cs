using Microsoft.AspNetCore.Mvc;

namespace ACCI_Center.Controllers.Binder
{
    public class FromJsonAttribute : ModelBinderAttribute
    {
        public FromJsonAttribute()
        {
            BinderType = typeof(JsonModelBinder);
        }
    }
}
