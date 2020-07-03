using UnityEngine.InputSystem;

namespace DynamicPixelCloud
{
    using UnityEngine;

    /// <summary>
    /// The SceneViewCameraController class.
    /// </summary>
    public class SceneViewCameraController : MonoBehaviour
    {
        /// <summary>
        /// The camera container gameobject.
        /// </summary>
        public GameObject parent;

        /// <summary>
        /// The target gameobject that the camera look at.
        /// </summary>
        public GameObject target;

        /// <summary>
        /// The camera move speed.
        /// </summary>
        public float cameraMoveSpeed = 1.0f;

        /// <summary>
        /// The camera zoom in/zoom out speed.
        /// </summary>
        public float cameraScaleSpeed = 3.0f;

        /// <summary>
        /// The camera rotation speed.
        /// </summary>
        public float cameraRotationSpeed = 0.1f;

        /// <summary>
        /// The camera min height.
        /// </summary>
        public float minHeight = 20f;

        /// <summary>
        /// The camera max height.
        /// </summary>
        public float maxHeight = 60f;

        /// <summary>
        /// The main camera.
        /// </summary>
        private Camera mainCamera;

        /// <summary>
        /// The camera original position.
        /// </summary>
        private Vector3 cameraOriginalPosition;

        /// <summary>
        /// The mouse original position used by camera movement.
        /// </summary>
        private Vector3 mouseOriginalPosition;

        /// <summary>
        /// The mouse current position used by camera movement.
        /// </summary>
        private Vector3 mouseCurrentPosition;

        /// <summary>
        /// The mouse original position used by camera rotation.
        /// </summary>
        private Vector3 mouse2OriginalPosition;

        /// <summary>
        /// The mouse current position used by camera rotation.
        /// </summary>
        private Vector3 mouse2CurrentPosition;

        /// <summary>
        /// Enters the Awake stage.
        /// </summary>
        private void Awake()
        {
            this.mainCamera = Camera.main;
            cameraOriginalPosition = this.parent.transform.position;
        }

        /// <summary>
        /// Enters the LateUpdate stage.
        /// </summary>
        private void LateUpdate()
        {
            this.UpdateCameraPosition();
            this.UpdateCameraHeight();
            this.UpdateCameraRotation();
        }

        /// <summary>
        /// Updates the camera position.
        /// </summary>
        private void UpdateCameraPosition()
        {
            // if (Mouse.current.rightButton.isPressed)
            // {
            //     mouseOriginalPosition = Mouse.current.delta.ReadValue();
            //     cameraOriginalPosition = this.parent.transform.position;
            // }

            if (Mouse.current.rightButton.isPressed)
            {
                mouseCurrentPosition = Mouse.current.delta.ReadValue();
                var mouseMove = mouseCurrentPosition - mouseOriginalPosition;
                var cameraMove = (mouseMove.x * this.mainCamera.transform.right + mouseMove.y * this.mainCamera.transform.forward);
                cameraMove.y = 0f;
                var targetPosition = cameraOriginalPosition - cameraMove * this.cameraMoveSpeed * Time.fixedDeltaTime;
                this.parent.transform.position = targetPosition;
                mouseOriginalPosition = mouseCurrentPosition;
                cameraOriginalPosition = this.parent.transform.position;
            }
        }

        /// <summary>
        /// Updates the camera field of view.
        /// </summary>
        private void UpdateCameraHeight()
        {
            var mouseScrollView = Mouse.current.scroll.y.ReadValue();
            var direction = (target.transform.position - this.mainCamera.transform.position).normalized;
            var targetPosition = this.mainCamera.transform.position + direction * mouseScrollView * this.cameraScaleSpeed * Time.fixedDeltaTime;
            var distance = Mathf.Clamp((targetPosition - target.transform.position).magnitude, this.minHeight, this.maxHeight);
            this.mainCamera.transform.position = target.transform.position - direction * distance;
        }

        /// <summary>
        /// Updates the camera rotation.
        /// </summary>
        private void UpdateCameraRotation()
        {
            // if (Mouse.current.middleButton.wasPressedThisFrame)
            // {
            //     mouse2OriginalPosition = Mouse.current.delta.ReadValue();
            // }

            if (Mouse.current.middleButton.isPressed)
            {
                mouse2CurrentPosition = Mouse.current.delta.ReadValue();
                var mouseMove = mouse2CurrentPosition.x - mouse2OriginalPosition.x;
                Camera.main.transform.RotateAround(target.transform.position, Vector3.up, mouseMove * this.cameraRotationSpeed * Time.fixedDeltaTime);
                mouse2OriginalPosition = mouse2CurrentPosition;
            }
        }
    }
}
