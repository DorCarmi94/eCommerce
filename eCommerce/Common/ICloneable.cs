namespace eCommerce.Common
{
    public interface ICloneable<T>
    {
        /// <summary>
        /// Returns a deep copy of the object
        /// </summary>
        /// <returns>Deep copy of the object</returns>
        public T Clone();
    }
}