using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class edit_scene_ani : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private TextMeshProUGUI project_path;

    void Update()
    {
        if (project_path.text.Length <= 0 || project_path.text == main.instance.defualt_path) {
            UpdateAnimatorParameter(animator, "get_value", AnimatorControllerParameterType.Bool, false);
        }
        else if (project_path.text.Length > 0 || project_path.text != main.instance.defualt_path)
        {
            UpdateAnimatorParameter(animator, "get_value", AnimatorControllerParameterType.Bool, true);
            if (main.instance.og_block_path.Length > 0)
            {
                UpdateAnimatorParameter(animator, "fail", AnimatorControllerParameterType.Bool, false);
            }
            else if (main.instance.og_block_path.Length <= 0)
            {
                UpdateAnimatorParameter(animator, "fail", AnimatorControllerParameterType.Bool, true);
            }
        }
    }
    private void UpdateAnimatorParameter(Animator animator, string parameterName, AnimatorControllerParameterType parameterType, object value)
    {
        switch (parameterType)
        {
            case AnimatorControllerParameterType.Bool:
                animator.SetBool(parameterName, (bool)value);
                break;

            case AnimatorControllerParameterType.Int:
                animator.SetInteger(parameterName, (int)value);
                break;

            case AnimatorControllerParameterType.Float:
                animator.SetFloat(parameterName, (float)value);
                break;

            case AnimatorControllerParameterType.Trigger:
                if ((bool)value)
                    animator.SetTrigger(parameterName);
                else
                    animator.ResetTrigger(parameterName);
                break;

            default:
                Debug.LogWarning("Unsupported parameter type.");
                break;
        }
    }
}
