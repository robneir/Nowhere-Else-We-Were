using UnityEngine;
using System.Collections;

public class CharacterAnimationEventHandler : MonoBehaviour {

	[SerializeField]
	private GameManager gm;
	[SerializeField]
	private AudioClip walkSoundLeft;
	[SerializeField]
	private AudioClip walkSoundRight;
    [SerializeField]
    private AudioClip attackSound;
    [SerializeField]
    private AudioClip hitSound;

    // Play walking sound either right or left foot.
    public void PlayWalkSound(string whichFoot)
	{
		if (whichFoot == "Right") 
		{
			if (walkSoundRight != null) 
			{
				gm.audioManager.Play2DSound (walkSoundRight, .6f, false);
			}
		} 
		else if(whichFoot == "Left")
		{
			if (walkSoundLeft != null) 
			{
				gm.audioManager.Play2DSound (walkSoundLeft, .6f, false);
			}
		}
	}

    public void PlayAttackSound()
    {
        if(attackSound != null)
        {
            gm.audioManager.Play2DSound(attackSound, 1.0f, false);
        }
    }

    public void PlayHitSound()
    {
        if (hitSound != null)
        {
            gm.audioManager.Play2DSound(hitSound, 1.0f, false);
        }
    }
}
