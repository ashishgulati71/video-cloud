using UnityEngine;
using System.Collections;
using Vuforia;
using UnityEngine.UI;

public class SimpleCloudHandler : MonoBehaviour , ICloudRecoEventHandler{
//	public ImageTargetBehaviour ImageTargetTemplate;
	private CloudRecoBehaviour mCloudRecoBehaviour;
	private bool mIsScanning = false;
	private string mTargetMetadata = "";

	[Space(10)]
	public GameObject GO_Video;

	[Space(10)]
	public Text debugtext;

	[Space(10)]
	public GameObject GO_Scanning;
	public GameObject GO_ResetButton;


	public ImageTargetBehaviour ImageTargetTemplate;
	
	void Start () {	

		GO_ResetButton.SetActive (false);
		mCloudRecoBehaviour = GetComponent<CloudRecoBehaviour>();
		if(mCloudRecoBehaviour){
			mCloudRecoBehaviour.RegisterEventHandler(this);
		}
	}
	
	public void OnInitialized() {
		Debug.Log ("Cloud Reco initialized");
	}
	public void OnInitError(TargetFinder.InitState initError) {
		Debug.Log ("Cloud Reco init error " + initError.ToString());
	}
	public void OnUpdateError(TargetFinder.UpdateState updateError) {
		Debug.Log ("Cloud Reco update error " + updateError.ToString());
	}
	
	public void OnStateChanged(bool scanning) {
		mIsScanning = scanning;
		if (scanning)
		{
			ObjectTracker tracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
			tracker.TargetFinder.ClearTrackables(false);
		}
	}

	public void OnNewSearchResult(TargetFinder.TargetSearchResult targetSearchResult) {
		mTargetMetadata = targetSearchResult.MetaData;
		mCloudRecoBehaviour.CloudRecoEnabled = false;

		//Video Path Set Here
		string ggwp = "https://s3.ap-south-1.amazonaws.com/unityvideo/sample10.mp4";
		GO_Video.GetComponent<VideoPlaybackBehaviour> ().changed_videopath = ggwp;
		debugtext.text = mTargetMetadata.ToString();


		// Build augmentation based on target
		if (ImageTargetTemplate) {
			// enable the new result with the same ImageTargetBehaviour:
			ObjectTracker tracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
			ImageTargetBehaviour imageTargetBehaviour =
				(ImageTargetBehaviour)tracker.TargetFinder.EnableTracking(
					targetSearchResult, ImageTargetTemplate.gameObject);
		}
	}
	
	void OnGUI() {
		// Display current 'scanning' status

		if (mIsScanning) {
			GO_Scanning.SetActive (true);
			GO_ResetButton.SetActive (false);
		} else {
			GO_Scanning.SetActive (false);
			GO_ResetButton.SetActive (true);
		}

		// Display metadata of latest detected cloud-target
		//GUI.Box (new Rect(100,200,200,50), "Metadata: " + mTargetMetadata);

		// If not scanning, show button
		// so that user can restart cloud scanning

		if (!mIsScanning) {
			GO_ResetButton.SetActive (true);
		}


		/*
		if (!mIsScanning) {
			if (GUI.Button(new Rect(100,300,200,50), "Restart Scanning")) {
				// Restart TargetFinder
				mCloudRecoBehaviour.CloudRecoEnabled = true;
			}
		}
		*/
	}

	public void ResetScanning(){
		mCloudRecoBehaviour.CloudRecoEnabled = true;
	}
}
