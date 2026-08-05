using System;
using UnityEngine;

public class ActorDebugAnimationElement : BaseDebugAnimationElement<ActorAsset>
{
	public SpriteAnimation adult;

	public SpriteAnimation baby;

	protected override void Start()
	{
		base.Start();
		adult.create();
		baby.create();
	}

	public override void update()
	{
		if (is_playing)
		{
			adult.update(Time.deltaTime);
			if (asset.has_baby_form)
			{
				baby.update(Time.deltaTime);
			}
			frame_number_text.text = adult.currentFrameIndex.ToString();
		}
	}

	public override void setData(ActorAsset pAsset)
	{
		base.setData(pAsset);
		if (!asset.has_baby_form)
		{
			baby.enabled = false;
			baby.image.sprite = null;
			baby.image.color = Color.clear;
		}
	}

	protected override void clear()
	{
		adult.enabled = true;
		adult.image.color = Color.white;
		adult.frames = Array.Empty<Sprite>();
		adult.resetAnim();
		baby.enabled = true;
		baby.image.color = Color.white;
		baby.frames = Array.Empty<Sprite>();
		baby.resetAnim();
	}

	public override void stopAnimations()
	{
		base.stopAnimations();
		adult.isOn = false;
		baby.isOn = false;
		frame_number_text.text = adult.currentFrameIndex.ToString();
	}

	public override void startAnimations()
	{
		base.startAnimations();
		adult.isOn = true;
		baby.isOn = true;
	}

	protected override void clickNextFrame()
	{
		if (!is_playing)
		{
			int tFramesCount = adult.frames.Length;
			adult.currentFrameIndex++;
			baby.currentFrameIndex++;
			if (adult.currentFrameIndex > tFramesCount - 1)
			{
				adult.currentFrameIndex = 0;
				baby.currentFrameIndex = 0;
			}
			frame_number_text.text = adult.currentFrameIndex.ToString();
			adult.updateFrame();
			baby.updateFrame();
		}
	}
}
