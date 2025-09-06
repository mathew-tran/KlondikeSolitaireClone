using UnityEngine;

public class SFXManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource FlipCardSFX;

    [SerializeField]
    private AudioSource PlaceCardSFX;

    [SerializeField]
    private AudioSource DragCardSFX;

    public static SFXManager mInstance;

    public static SFXManager GetInstance()
    {
        if (mInstance == null)
        {
            GameObject obj = Resources.Load<GameObject>("SFXManager");
            Instantiate(obj);
        }
        return mInstance;
    }
    private void Awake()
    {
        if (mInstance)
        {
            Destroy(this.gameObject);
            return;
        }
        mInstance = this;
        DontDestroyOnLoad(this.gameObject);

    }

    public void StopAllSounds()
    {
        DragCardSFX.Stop();
        PlaceCardSFX.Stop();
        FlipCardSFX.Stop();
    }
    public void PlayDragSFX(Vector3 pos)
    {
        if (Settings.GetInstance().GetPlaySFX() == false)
        {
            return;
        }
        StopAllSounds();
        DragCardSFX.pitch = Random.Range(.8f, 1.2f);
        DragCardSFX.gameObject.transform.position = pos;
        DragCardSFX.Play();
    }

    public void PlayCardPlaceSFX(Vector3 pos)
    {
        if (Settings.GetInstance().GetPlaySFX() == false)
        {
            return;
        }

        StopAllSounds();
        DragCardSFX.pitch = Random.Range(1f, 1.2f);
        PlaceCardSFX.gameObject.transform.position = pos;
        PlaceCardSFX.Play();
    }

    public void PlayFlipCardSFX(Vector3 pos)
    {
        if (Settings.GetInstance().GetPlaySFX() == false)
        {
            return;
        }

        StopAllSounds();
        DragCardSFX.pitch = Random.Range(.6f, .8f);
        FlipCardSFX.gameObject.transform.position = pos;
        FlipCardSFX.Play();
    }
}
