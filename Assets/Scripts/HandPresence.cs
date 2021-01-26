using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandPresence : MonoBehaviour
{
    public bool showController = false;
    public InputDeviceCharacteristics ControllerCharacteristics;
    public List<GameObject> ControllerPrefabs;
    public GameObject HandModelPrefab;

    private InputDevice _targetDevice;
    private GameObject _spawnedController;
    private GameObject _spawnedHandModel;
    private Animator _handAnimator;
    public Animator HandAnimator => _handAnimator;

    [SerializeField]
    private Transform _parent;

    // Start is called before the first frame update
    void Start()
    {
        TryInitialize();
    }

    internal void TryInitialize()
    {
        List<InputDevice> devices = new List<InputDevice>();

        InputDevices.GetDevicesWithCharacteristics(ControllerCharacteristics, devices);

        foreach (var item in devices)
        {
            Debug.Log(item.name + item.characteristics);
        }

        if (devices.Count > 0)
        {
            _targetDevice = devices[0];

            if (showController)
            {
                string targetName = _targetDevice.name;
                // In case were using OpenXR
                int openXrIndex = _targetDevice.name.IndexOf("OpenXR");
                if (openXrIndex >= 0 && openXrIndex < _targetDevice.name.Length)
                {
                    targetName = targetName.Substring(0, openXrIndex - 1);
                    Debug.LogWarning(targetName);
                }

                GameObject prefab = ControllerPrefabs.Find(controller => controller.name.Contains(targetName));

                if (prefab)
                {
                    _spawnedController = Instantiate(prefab, transform);
                }
                else
                {
                    Debug.LogWarning("Did not find corresponding controller model.");
                    _spawnedController = Instantiate(ControllerPrefabs[0], transform);
                }
            }
            
            if(!_spawnedHandModel)
                _spawnedHandModel = Instantiate(HandModelPrefab, _parent);

            _handAnimator = _spawnedHandModel.GetComponent<Animator>();
        }
    }

    void UpdateHandAnimation()
    {
        if (_targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
        {
            _handAnimator.SetFloat("Trigger", triggerValue);
        }
        else
        {
            _handAnimator.SetFloat("Trigger", 0);
        }
        if (_targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
        {
            _handAnimator.SetFloat("Grip", gripValue);
        }
        else
        {
            _handAnimator.SetFloat("Grip", 0);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if (!_targetDevice.isValid)
        {
            TryInitialize();
        }
        else
        {
            //if (showController)
            //{
            //    _spawnedController.SetActive(true);
            //}
            //else
            //{
            //    _spawnedController.SetActive(false);
            //}

            UpdateHandAnimation();
        }
    }

    private void LateUpdate()
    {
        GameObject xr = transform.parent.transform.parent.gameObject;

        //xr.transform.SetPositionAndRotation(
        //    xr.transform.position,
        //    xr.transform.rotation * Quaternion.Euler(45, 0, 0)
        //    );
    }
}
