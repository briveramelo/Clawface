using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModMan;

public class EnemyOutline : MonoBehaviour
{
    const float FADE_SPEED = 0.15f;

    [SerializeField] OutlineMode mode;
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] EnemyBase enemy;

    GameObject player;
    PlayerStateManager playerState;

    private void OnEnable()
    {
        switch (mode)
        {
            case OutlineMode.AlwaysOn:
                break;
            case OutlineMode.ShowWhenEdible:
                StartCoroutine(ShowWhenEdibleCoroutine());
                break;
        }
    }

    IEnumerator ShowWhenEdibleCoroutine ()
    {
        while (true)
        {
            if (enemy.IsEatable())
            {
                if (player == null)
                {
                    player = GameObject.FindGameObjectWithTag(Strings.Tags.PLAYER);
                }
                
                else
                {
                    if (playerState == null)
                    {
                        playerState = player.GetComponent<PlayerStateManager>();
                    }

                    else
                    {
                        float eatRadius = playerState.stateVariables.eatRadius;
                        float dist = Vector3.Distance (enemy.transform.position, player.transform.position);
                        if (dist <= eatRadius)
                        {
                            sprite.SetAlpha(Mathf.Clamp01(sprite.color.a + FADE_SPEED));
                        }
                        else
                        {
                            sprite.SetAlpha(Mathf.Clamp01(sprite.color.a - FADE_SPEED));
                        }
                    }
                }
            }

            else
            {
                sprite.SetAlpha(0.0f);
            }

            yield return null;
        }
    }

    public enum OutlineMode
    {
        AlwaysOn,
        ShowWhenEdible
    }
}
