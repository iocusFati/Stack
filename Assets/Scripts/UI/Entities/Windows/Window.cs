using UI.Factory;
using UnityEngine;

namespace Infrastructure.States
{
    public abstract class Window : MonoBehaviour, IUIEntity
    {
        public virtual void Show() => 
            gameObject.SetActive(true);

        public virtual void Hide() => 
            gameObject.SetActive(false);
    }
}