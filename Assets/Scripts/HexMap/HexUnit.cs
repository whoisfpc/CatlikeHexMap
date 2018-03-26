using UnityEngine;

namespace HexMap
{
    public class HexUnit : MonoBehaviour
    {
        private HexCell location;
        public HexCell Location
        {
            get
            {
                return location;
            }
            set
            {
                location = value;
                value.Unit = this;
                transform.localPosition = value.Position;
            }
        }

        private float orientation;
        public float Orientation
        {
            get
            {
                return orientation;
            }
            set
            {
                orientation = value;
                transform.localRotation = Quaternion.Euler(0, value, 0);
            }
        }

        public void ValidateLocation()
        {
            transform.localPosition = location.Position;
        }

        public void Die()
        {
            location.Unit = null;
            Destroy(gameObject);
        }
    }
}
