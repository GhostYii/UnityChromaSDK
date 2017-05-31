﻿using ChromaSDK.ChromaPackage.Model;
using RazerSDK.RazerPackage.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using ChromaApi = ChromaSDK.Api.DefaultApi;
using ChromaCustomApi = CustomChromaSDK.Api.DefaultApi;
using RazerApi = RazerSDK.Api.DefaultApi;
using CustomEffectType = CustomChromaSDK.CustomChromaPackage.Model.EffectType;
using CustomKeyboardInput = CustomChromaSDK.CustomChromaPackage.Model.KeyboardInput;

public class TestSwaggerClient : MonoBehaviour
{
    /// <summary>
    /// Meta reference to a ui button
    /// </summary>
    public Button _mButtonAllBlue;

    /// <summary>
    /// Meta reference to a ui button
    /// </summary>
    public Button _mButtonAllGreen;

    /// <summary>
    /// Meta reference to a ui button
    /// </summary>
    public Button _mButtonAllRed;

    /// <summary>
    /// Meta reference to a ui button
    /// </summary>
    public Button _mButtonAllOrange;

    /// <summary>
    /// Meta reference to a ui button
    /// </summary>
    public Button _mButtonAllWhite;

    /// <summary>
    /// Meta reference to a ui button
    /// </summary>
    public Button _mButtonCustom;

    /// <summary>
    /// Meta reference to a ui button
    /// </summary>
    public Button _mButtonAllClear;

    /// <summary>
    /// Instance of the RazerAPI
    /// </summary>
    private RazerApi _mApiRazerInstance;

    /// <summary>
    /// Instance of the API
    /// </summary>
    private ChromaApi _mApiInstance;

    /// <summary>
    /// Instance of the custom API
    /// </summary>
    private ChromaCustomApi _mApiCustomInstance;

    /// <summary>
    /// Initialize Chroma by hitting the REST server and set the API port
    /// </summary>
    /// <returns></returns>
    void InitChroma()
    {
        try
        {
            // use the Razer API to get the session
            _mApiRazerInstance = new RazerApi();

            var input = new ChromaSdkInput();
            input.Title = "Test";
            input.Description = "This is a REST interface test application";
            input.Author = new ChromaSdkInputAuthor();
            input.Author.Name = "Chroma Developer";
            input.Author.Contact = "www.razerzone.com";
            input.DeviceSupported = new List<string> 
            {
                "keyboard",
                "mouse",
                "headset",
                "mousepad",
                "keypad",
                "chromalink",
            };
            input.Category = "application";
            ChromaSdkResponse result = _mApiRazerInstance.Chromasdk(input);
            Debug.Log(result);

            // setup the api instances with the session uri
            _mApiInstance = new ChromaApi(result.Uri);
            _mApiCustomInstance = new ChromaCustomApi(result.Uri);
        }
        catch (Exception e)
        {
            Debug.LogFormat("Exception when calling RazerApi.Chromasdk: {0}", e);
        }
    }

    /// <summary>
    /// Use API to set the CHROMA_NONE effect
    /// </summary>
    void ClearEffect()
    {
        try
        {
            var input = new KeyboardInput();
            input.Effect = EffectType.CHROMA_NONE;
            KeyboardResponse result = _mApiInstance.PutKeyboard(input);
            Debug.Log(result);
        }
        catch (Exception e)
        {
            Debug.LogFormat("Exception when calling ChromaApi.PutKeyboard: {0}", e);
        }
    }

    /// <summary>
    /// Use the API to set the CHROMA_STATIC effect
    /// </summary>
    /// <param name="color"></param>
    void SetStaticColor(int color)
    {
        try
        {
            var input = new KeyboardInput();
            input.Effect = EffectType.CHROMA_STATIC;
            input.Param = new KeyboardInputParam(color);
            KeyboardResponse result = _mApiInstance.PutKeyboard(input);
            Debug.Log(result);
        }
        catch (Exception e)
        {
            Debug.LogFormat("Exception when calling ChromaApi.PutKeyboard: {0}", e);
        }
    }

    /// <summary>
    /// Use the API to set the CHROMA_CUSTOM effect
    /// </summary>
    void SetCustomEffect()
    {
        var rows = new List<List<int?>>();
        for (int i = 0; i < 6; ++i)
        {
            var row = new List<int?>();
            for (int j = 0; j < 22; ++j)
            {
                row.Add(UnityEngine.Random.Range(0, 16777215));
            }
            rows.Add(row);
        }
        try
        {
            var input = new CustomKeyboardInput(CustomEffectType.CHROMA_CUSTOM, rows);
            _mApiCustomInstance.PutKeyboard(input);
        }
        catch (Exception e)
        {
            Debug.LogFormat("Exception when calling ChromaCustomApi.PutKeyboard: {0}", e);
        }
    }

    /// <summary>
    /// Use heartbeat to keep the REST API listening after initialization
    /// </summary>
    /// <returns></returns>
    IEnumerator HeartBeat()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);

            // only one heartbeat is needed
            // since the custom api hits the same port
            _mApiInstance.Heartbeat();
        }
    }

    // Use this for initialization
    void Start()
    {
        InitChroma();

        // use heartbeat to keep the REST API alive
        StartCoroutine(HeartBeat());

        // subscribe to ui click events
        _mButtonAllBlue.onClick.AddListener(() =>
        {
            SetStaticColor(16711680);
        });

        // subscribe to ui click events
        _mButtonAllGreen.onClick.AddListener(() =>
        {
            SetStaticColor(65280);
        });

        // subscribe to ui click events
        _mButtonAllRed.onClick.AddListener(() =>
        {
            SetStaticColor(255);
        });

        // subscribe to ui click events
        _mButtonAllOrange.onClick.AddListener(() =>
        {
            SetStaticColor(35071);
        });

        // subscribe to ui click events
        _mButtonAllWhite.onClick.AddListener(() =>
        {
            SetStaticColor(16777215);
        });

        // subscribe to ui click events
        _mButtonCustom.onClick.AddListener(() =>
        {
            SetCustomEffect();
        });

        // subscribe to ui click events
        _mButtonAllClear.onClick.AddListener(() =>
        {
            ClearEffect();
        });

    }

    /// <summary>
    /// Clear the active effect on quit
    /// </summary>
    private void OnApplicationQuit()
    {
        ClearEffect();
    }
}
