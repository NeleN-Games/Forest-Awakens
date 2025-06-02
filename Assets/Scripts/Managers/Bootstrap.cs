using Databases;
using Services;
using UnityEngine;

namespace Managers
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField] private SourceDatabase sourceDatabase;
        [SerializeField] private ItemDatabase itemDatabase;

        private void Awake()
        {
            ServiceLocator.Register(sourceDatabase);
            ServiceLocator.Register(itemDatabase);
        }
    }
}