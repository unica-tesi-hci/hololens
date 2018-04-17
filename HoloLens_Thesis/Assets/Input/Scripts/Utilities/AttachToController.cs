﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA.Input;
#else
using UnityEngine.VR.WSA.Input;
#endif

/// <summary>
    /// Waits for a controller to be instantiated, then attaches itself to a specified element
    /// </summary>
    public class AttachToController : MonoBehaviour
    {
/*        public InteractionSourceHandedness Handedness { get { return handedness; } }

        public MotionControllerInfo.ControllerElementEnum Element { get { return element; } }

        [Header("AttachToController Elements")]
        [SerializeField]
        protected InteractionSourceHandedness handedness = InteractionSourceHandedness.Left;

        [SerializeField]
        protected MotionControllerInfo.ControllerElementEnum element = MotionControllerInfo.ControllerElementEnum.PointingPose;

        public bool SetChildrenInactiveWhenDetached = true;

        [SerializeField]
        protected Vector3 positionOffset = Vector3.zero;

        [SerializeField]
        protected Vector3 rotationOffset = Vector3.zero;

        [SerializeField]
        protected Vector3 scale = Vector3.one;

        [SerializeField]
        protected bool setScaleOnAttach = false;

        public bool IsAttached { get; private set; }

        private Transform elementTransform;
        public Transform ElementTransform { get; private set; }

        protected MotionControllerInfo controller;

        protected virtual void OnAttachToController() { }
        protected virtual void OnDetachFromController() { }

        protected virtual void OnEnable()
        {
            SetChildrenActive(false);

#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            // Look if the controller has loaded.
            if (MotionControllerVisualizer.Instance.TryGetControllerModel(handedness, out controller))
            {
                AttachElementToController(controller);
            }
#endif

            MotionControllerVisualizer.Instance.OnControllerModelLoaded += AttachElementToController;
            MotionControllerVisualizer.Instance.OnControllerModelUnloaded += DetachElementFromController;
        }

        protected virtual void OnDisable()
        {
            if (MotionControllerVisualizer.IsInitialized)
            {
                MotionControllerVisualizer.Instance.OnControllerModelLoaded -= AttachElementToController;
                MotionControllerVisualizer.Instance.OnControllerModelUnloaded -= DetachElementFromController;
            }
        }

        protected virtual void OnDestroy()
        {
            if (MotionControllerVisualizer.IsInitialized)
            {
                MotionControllerVisualizer.Instance.OnControllerModelLoaded -= AttachElementToController;
                MotionControllerVisualizer.Instance.OnControllerModelUnloaded -= DetachElementFromController;
            }
        }

        private void AttachElementToController(MotionControllerInfo newController)
        {
            if (!IsAttached && newController.Handedness == handedness)
            {
                if (!newController.TryGetElement(element, out elementTransform))
                {
                    Debug.LogError("Unable to find element of type " + element + " under controller " + newController.ControllerParent.name + "; not attaching.");
                    return;
                }

                controller = newController;

                SetChildrenActive(true);

                // Parent ourselves under the element and set our offsets
                transform.parent = elementTransform;
                transform.localPosition = positionOffset;
                transform.localEulerAngles = rotationOffset;
                if (setScaleOnAttach)
                {
                    transform.localScale = scale;
                }

                // Announce that we're attached
                OnAttachToController();

                IsAttached = true;
            }
        }

        private void DetachElementFromController(MotionControllerInfo oldController)
        {
            if (IsAttached && oldController.Handedness == handedness)
            {
                OnDetachFromController();

                controller = null;
                transform.parent = null;

                SetChildrenActive(false);

                IsAttached = false;
            }
        }

        private void SetChildrenActive(bool isActive)
        {
            if (SetChildrenInactiveWhenDetached)
            {
                foreach (Transform child in transform)
                {
                    child.gameObject.SetActive(isActive);
                }
            }
        }*/
    }