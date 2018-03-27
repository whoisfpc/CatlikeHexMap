using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using HexMap.Util;

namespace HexMap
{
    public class HexUnit : MonoBehaviour
    {
        private const float travelSpeed = 4f;
        private const float rotationSpeed = 180f;

        public static HexUnit unitPrefab;

        private HexCell location;
        public HexCell Location
        {
            get
            {
                return location;
            }
            set
            {
                if (location)
                {
                    location.Unit = null;
                }
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

        private List<HexCell> pathToTravel;

        public void ValidateLocation()
        {
            transform.localPosition = location.Position;
        }

        public void Travel(List<HexCell> path)
        {
            Location = path[path.Count - 1];
            pathToTravel = path;
            StopAllCoroutines();
            StartCoroutine(TravelPath());
        }

        private IEnumerator TravelPath()
        {
            Vector3 a, b, c = pathToTravel[0].Position;
            transform.localPosition = c;
            yield return LookAt(pathToTravel[1].Position);

            float t = Time.deltaTime * travelSpeed;
            for (int i = 1; i < pathToTravel.Count; i++)
            {
                a = c;
                b = pathToTravel[i - 1].Position;
                c = (b + pathToTravel[i].Position) * 0.5f;
                for (; t < 1f; t += Time.deltaTime * travelSpeed)
                {
                    transform.localPosition = Bezier.GetPoint(a, b, c, t);
                    Vector3 d = Bezier.GetFirstDerivative(a, b, c, t);
                    d.y = 0f;
                    transform.localRotation = Quaternion.LookRotation(d);
                    yield return null;
                }
                t -= 1f;
            }

            a = c;
            b = pathToTravel[pathToTravel.Count - 1].Position;
            c = b;
            for (; t < 1f; t += Time.deltaTime * travelSpeed)
            {
                transform.localPosition = Bezier.GetPoint(a, b, c, t);
                Vector3 d = Bezier.GetFirstDerivative(a, b, c, t);
                d.y = 0f;
                transform.localRotation = Quaternion.LookRotation(d);
                yield return null;
            }

            transform.localPosition = location.Position;
            orientation = transform.localRotation.eulerAngles.y;
            ListPool<HexCell>.Add(pathToTravel);
            pathToTravel = null;
        }

        IEnumerator LookAt(Vector3 point)
        {
            point.y = transform.localPosition.y;
            Quaternion fromRotation = transform.localRotation;
            Quaternion toRotation = Quaternion.LookRotation(point - transform.localPosition);
            float angle = Quaternion.Angle(fromRotation, toRotation);
            float speed = rotationSpeed / angle;

            if (angle > 0)
            {
                for (float t = Time.deltaTime * speed; t < 1f; t += Time.deltaTime * speed)
                {
                    transform.localRotation = Quaternion.Slerp(fromRotation, toRotation, t);
                    yield return null;
                }

                transform.LookAt(point);
                orientation = transform.localRotation.eulerAngles.y;
            }
        }

        public void Die()
        {
            location.Unit = null;
            Destroy(gameObject);
        }

        public void Save(BinaryWriter writer)
        {
            location.coordinates.Save(writer);
            writer.Write(orientation);
        }

        public bool IsValidDestination(HexCell cell)
        {
            return !cell.IsUnderwater && !cell.Unit;
        }

        public static void Load(BinaryReader reader, HexGrid grid)
        {
            HexCoordinates coordinates = HexCoordinates.Load(reader);
            float orientation = reader.ReadSingle();
            grid.AddUnit(Instantiate(unitPrefab), grid.GetCell(coordinates), orientation);
        }

        private void OnEnable()
        {
            if (location)
            {
                transform.localPosition = location.Position;
            }
        }
    }
}
