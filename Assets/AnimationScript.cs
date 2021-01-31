using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScript : MonoBehaviour
{
    public static int N_DIRECTIONS = 4, N_WALK_FRAMES = 3;
    public static float walkTimeBetweenFrames = 0.3f;
    public static int IDLE_FRAME = 0;
    public static int DOWN = 3, RIGHT = 2, UP = 1, LEFT = 0; 

    public Texture2D spriteSheet;

    int frameWidth, frameHeight;
    float timeTilNextFrame = 0;
    bool walking = false;
    int direction;
    int curFrame = 0;

    SpriteRenderer renderObj;

    // Start is called before the first frame update
    void Start()
    {
        frameWidth = spriteSheet.width / 4;
        frameHeight = spriteSheet.height / 4;
        renderObj = gameObject.GetComponent<SpriteRenderer>();
        SetSprite(DOWN, IDLE_FRAME);
    }

    public void SetSprite(int direction, int frame)
    {
        curFrame = frame;
        renderObj.sprite = Sprite.Create(spriteSheet, new Rect(curFrame * frameWidth, direction * frameHeight, frameWidth, frameHeight), new Vector2(0.5f, 0.5f));
    }

    public void StartWalk(int newDirection)
    {
        if(!walking || direction != newDirection)
        {
            direction = newDirection;
            walking = true;
            curFrame = 1;
            SetSprite(direction, 1);
            timeTilNextFrame = walkTimeBetweenFrames;
        }
    }

    public void StopWalk()
    {
        if (walking)
        {
            walking = false;
            SetSprite(direction, IDLE_FRAME);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(walking)
        {
            if(timeTilNextFrame <= 0)
            {
                timeTilNextFrame = walkTimeBetweenFrames;
                curFrame++;
                if(curFrame >= 4)
                {
                    curFrame = 1;
                }
                SetSprite(direction, curFrame);
            }
            else
            {
                timeTilNextFrame -= Time.deltaTime;
            }
        }
    }
}
