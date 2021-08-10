namespace UserManagement.Domain
{
    /// <summary>
    /// Interface IEntity
    /// </summary>
    public interface IEntity
    {
    }
    /// <summary>
    /// Class BaseEntity.
    /// </summary>
    /// <seealso cref="UserManagement.Domain.IEntity" />
    public abstract class BaseEntity : IEntity
    {
    }
    /// <summary>
    /// Class BaseEntity.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="UserManagement.Domain.BaseEntity" />
    /// <seealso cref="UserManagement.Domain.IEntity" />
    public abstract class BaseEntity<T> : BaseEntity, IEntity
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        public abstract T Key { get; set; }
    }

}
