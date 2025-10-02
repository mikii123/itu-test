using UnityEngine;

namespace ITU.View
{
	public class CameraController : MonoBehaviour
	{
		public static CameraController Instance;
		public Camera Camera { get; private set; }

		[SerializeField] private float rotationSensitivity = 120;
		[SerializeField] private float moveSensitivity = 40;
		[SerializeField] private float zoomSensitivity = 120;

		private void Awake()
		{
			Instance = this;
			Camera = GetComponent<Camera>();
		}

		private void Update()
		{
			if (Input.GetMouseButton(1))
			{
				var mouseY = -Input.GetAxis("Mouse Y") * rotationSensitivity;
				var mouseX = Input.GetAxis("Mouse X") * rotationSensitivity;
				transform.Rotate(mouseY * Time.deltaTime, mouseX * Time.deltaTime, 0);
				transform.rotation = Quaternion.LookRotation(transform.forward, Vector3.up);
			}

			if (Input.GetMouseButton(2))
			{
				var mouseY = -Input.GetAxis("Mouse Y") * moveSensitivity;
				var mouseX = -Input.GetAxis("Mouse X") * moveSensitivity;
				transform.position += transform.up * mouseY * Time.deltaTime;
				transform.position += transform.right * mouseX * Time.deltaTime;
			}

			if (Input.GetAxis("Mouse ScrollWheel") != 0)
			{
				var mouseWheel = Input.GetAxis("Mouse ScrollWheel") * zoomSensitivity;
				transform.position += transform.forward * mouseWheel * Time.deltaTime;
			}
		}
	}
}
