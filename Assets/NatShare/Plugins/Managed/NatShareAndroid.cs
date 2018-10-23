﻿/* 
*   NatShare
*   Copyright (c) 2018 Yusuf Olokoba
*/

namespace NatShareU.Platforms {

	using UnityEngine;
	using System;

	public class NatShareAndroid : INatShare {

		private readonly AndroidJavaClass natshare;

		public NatShareAndroid () {
			natshare = new AndroidJavaClass("com.yusufolokoba.natshare.NatShare");
		}

		bool INatShare.Share (string text) {
			return natshare.CallStatic<bool>("shareText", text);
		}

		bool INatShare.Share (byte[] pngData, string message) {
			return natshare.CallStatic<bool>("shareImage", pngData, message);
		}

		bool INatShare.Share (string path, string message) {
			return natshare.CallStatic<bool>("shareMedia", path, message);
		}

		bool INatShare.SaveToCameraRoll (byte[] pngData) {
			return natshare.CallStatic<bool>("saveImageToCameraRoll", pngData);
		}

		bool INatShare.SaveToCameraRoll (string path) {
			return natshare.CallStatic<bool>("saveMediaToCameraRoll", path);
		}

		void INatShare.GetThumbnail (string videoPath, Action<Texture2D> callback, float time) {
			var thumbnail = natshare.CallStatic<AndroidJavaObject>("getThumbnail", videoPath, time);
            if (!thumbnail.Call<bool>("isLoaded")) {
                Debug.LogError("NatShare Error: Failed to get thumbnail for video at path: "+videoPath);
                callback(null);
            }
            var pixelData = AndroidJNI.FromByteArray(thumbnail
                .Get<AndroidJavaObject>("pixelBuffer")
                .Call<AndroidJavaObject>("array")
                .GetRawObject()
            );
            var image = new Texture2D(
                thumbnail.Get<int>("width"),
                thumbnail.Get<int>("height"),
                TextureFormat.RGB565,
                false
            );
            image.LoadRawTextureData(pixelData);
            image.Apply();
            callback(image);
		}
	}
}