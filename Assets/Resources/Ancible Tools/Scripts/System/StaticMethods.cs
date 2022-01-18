using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using AncibleCoreCommon.CommonData;
using AncibleCoreCommon.CommonData.Combat;
using Assets.Ancible_Tools.Scripts.Hitbox;
using MessageBusLib;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Assets.Ancible_Tools.Scripts.System
{
    public static class StaticMethods
    {
        public static List<T> QueryList<T>(this List<T> list, Predicate<T> query)
        {
            var returnList = new List<T>();
            for (var i = 0; i < list.Count; i++)
            {
                if (query.Invoke(list[i]))
                {
                    returnList.Add(list[i]);
                }
            }
            return returnList;
        }

        public static NullValue<T> QuerySingle<T>(this List<T> list, Predicate<T> query)
        {
            var returnValue = new NullValue<T>();
            for (var i = 0; i < list.Count; i++)
            {
                if (query.Invoke(list[i]))
                {
                    returnValue.SetValue(list[i]);
                    break;
                }
            }
            return returnValue;

        }

        public static void LerpMove(this Rigidbody2D rigidbody, float moveSpeed, float interpolation, Vector2 direction)
        {
            var position = rigidbody.position;
            position += Vector2.ClampMagnitude(moveSpeed * direction, moveSpeed);
            position = Vector2.Lerp(rigidbody.position, position, interpolation);
            //position.x = (float)Math.Round(position.x, DataController.ROUND_DECIMAL_PLACES);
            //position.y = (float)Math.Round(position.y, DataController.ROUND_DECIMAL_PLACES);
            rigidbody.position = position;
        }

        public static IEnumerator WaitForFrames(int frames, Action doAfter)
        {
            var frameCount = Time.frameCount + frames;
            yield return new WaitUntil(() =>
            {
                var currentFrameCount = Time.frameCount;
                return currentFrameCount >= frameCount;
            });
            doAfter.Invoke();
        }

        public static Vector2 ToVector2(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.y);
        }

        public static HitboxController SetupHitbox(this GameObject gameObject, Hitbox.Hitbox hitbox, CollisionLayer layer)
        {
            var hitboxFilter = HitboxController.GenerateHitboxFilter(hitbox, layer);
            var hitboxCheckmsg = MessageFactory.GenerateHitboxCheckMsg();
            HitboxController controller = null;
            hitboxCheckmsg.DoAfter = hitboxController =>
            {
                controller = hitboxController;
            };
            gameObject.SendMessageWithFilterTo(hitboxCheckmsg, gameObject.transform.parent.gameObject, hitboxFilter);
            MessageFactory.CacheMessage(hitboxCheckmsg);
            if (!controller)
            {
                controller = Object.Instantiate(hitbox.Controller, gameObject.transform.parent);
                controller.Setup(hitbox, layer);
            }
            return controller;
        }

        public static Vector2IntData ToData(this Vector2Int vector)
        {
            return new Vector2IntData(vector.x, vector.y);
        }

        public static Vector2Int ToVector(this Vector2IntData data)
        {
            return new Vector2Int(data.X, data.Y);
        }

        public static float GetHeightOfText(this Text uiObject, string text)
        {
            var generator = new TextGenerator();
            var settings = uiObject.GetGenerationSettings(uiObject.rectTransform.rect.size);
            settings.verticalOverflow = VerticalWrapMode.Overflow;
            settings.fontSize = uiObject.fontSize;
            settings.font = uiObject.font;
            settings.lineSpacing = uiObject.lineSpacing;
            generator.Populate(text, settings);
            //Debug.Log($"Line Count: {generator.lines.Count}");
            return (generator.lines.Count * settings.lineSpacing) * (settings.fontSize);
            //return generator.GetPreferredHeight(text, settings) /*/ (settings.font.fontSize / (float)settings.fontSize)*/;
        }

        public static float GetWidthOfText(this Text uiObject, string text)
        {
            var generator = new TextGenerator();
            var settings = uiObject.GetGenerationSettings(uiObject.rectTransform.rect.size);
            settings.horizontalOverflow = uiObject.horizontalOverflow;
            settings.fontSize = uiObject.fontSize;
            settings.lineSpacing = uiObject.lineSpacing;
            generator.Populate(text, settings);
            return generator.GetPreferredWidth(text, settings) / (settings.font.fontSize / (float)settings.fontSize);
        }

        public static int GetAverage(this List<int> listOfInts)
        {
            if (listOfInts.Count > 0)
            {
                var total = 0;
                for (var i = 0; i < listOfInts.Count; i++)
                {
                    total += listOfInts[i];
                }

                return total / listOfInts.Count;
            }
            else
            {
                return 0;
            }

        }

        public static float GetAverage(this List<float> list)
        {
            if (list.Count > 0)
            {
                var total = 0f;
                for (var i = 0; i < list.Count; i++)
                {
                    total += list[i];
                }

                return total / list.Count;
            }
            else
            {
                return 0f;
            }

        }

        public static Vector2Int Normalize(this Vector2Int vector)
        {
            var returnVector = vector;
            if (vector.x > 0)
            {
                returnVector.x = 1;
            }
            else if (vector.x < 0)
            {
                returnVector.x = -1;
            }

            if (vector.y > 0)
            {
                returnVector.y = 1;
            }
            else if (vector.y < 0)
            {
                returnVector.y = -1;
            }

            return returnVector;

        }

        public static string ApplyColorToText(string text, Color color)
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{text}</color>";
        }

        public static void SetTransformPosition(this Transform transform, Vector2 position)
        {
            var pos = transform.position;
            pos.x = position.x;
            pos.y = position.y;
            transform.position = pos;
        }

        public static Vector2 GetMouseQuadrant(Vector2 mousePos)
        {
            var middleScreen = new Vector2(Screen.width / 2f, Screen.height / 2f);
            var quadrant = Vector2.zero;
            if (mousePos.x > middleScreen.x)
            {
                quadrant.x = 1;
            }

            if (mousePos.y > middleScreen.y)
            {
                quadrant.y = 1;
            }

            return quadrant;
        }

        public static string ToDescription(this CombatStats stats)
        {
            var descriptors = new List<string>();
            if (stats.Health > 0 || stats.Health < 0)
            {
                descriptors.Add($"{stats.Health:+#,###;-#,###,#} Health");//+#,###;-#,###,#
            }

            if (stats.Strength > 0 || stats.Strength < 0)
            {
                descriptors.Add($"{stats.Strength:+#,###;-#,###,#} Strength");
            }

            if (stats.Agility > 0 || stats.Agility < 0)
            {
                descriptors.Add($"{stats.Agility:+#,###;-#,###,#} Agility");
            }

            if (stats.Intelligence > 0 || stats.Intelligence < 0)
            {
                descriptors.Add($"{stats.Intelligence:+#,###;-#,###,#} Intelligence");
            }

            if (stats.Endurance > 0 || stats.Endurance < 0)
            {
                descriptors.Add($"{stats.Endurance:+#,###;-#,###,#} Endurance");
            }

            if (stats.Wisdom > 0 || stats.Wisdom < 0)
            {
                descriptors.Add($"{stats.Wisdom:+#,###;-#,###,#} Wisdom");
            }

            if (stats.Dexterity > 0 || stats.Dexterity < 0)
            {
                descriptors.Add($"{stats.Dexterity:+#,###;-#,###,#} Dexterity");
            }

            if (stats.PhysicalDefense > 0 || stats.PhysicalDefense < 0)
            {
                descriptors.Add($"{stats.PhysicalDefense:+#,###;-#,###,#} Physical Defense");
            }

            if (stats.MagicalDefense > 0 || stats.MagicalDefense < 0)
            {
                descriptors.Add($"{stats.MagicalDefense:+#,###;-#,###,#} Magical Defense");
            }

            if (stats.PhysicalCriticalStrike > 0 || stats.PhysicalCriticalStrike < 0)
            {
                descriptors.Add($"{stats.PhysicalCriticalStrike:+#,###;-#,###,#} Physical Crit");
            }

            if (stats.MagicalCriticalStrike > 0 || stats.MagicalCriticalStrike < 0)
            {
                descriptors.Add($"{stats.MagicalCriticalStrike:+#,###;-#,###,#} Magical Crit");
            }

            if (stats.DodgeRating > 0 || stats.DodgeRating < 0)
            {
                descriptors.Add($"{stats.DodgeRating:+#,###;-#,###,#} Dodge Rating");
            }


            var description = string.Empty;
            for (var i = 0; i < descriptors.Count; i++)
            {
                description = i == 0 ? descriptors[i] : $"{description}{Environment.NewLine}{descriptors[i]}";
            }

            return description;
        }

        public static string ToInputString(this Key key)
        {
            switch (key)
            {
                case Key.Digit0:
                    return "0";
                case Key.Digit1:
                    return "1";
                case Key.Digit2:
                    return "2";
                case Key.Digit3:
                    return "3";
                case Key.Digit4:
                    return "4";
                case Key.Digit5:
                    return "5";
                case Key.Digit6:
                    return "6";
                case Key.Digit7:
                    return "7";
                case Key.Digit8:
                    return "8";
                case Key.Digit9:
                    return "9";
                default:
                    return $"{key}";
            }
        }

        public static float GetAngle(this Vector2 origin, Vector2 destination)
        {
            var angle = destination - origin;
            return Mathf.Atan2(angle.y, angle.x) * Mathf.Rad2Deg;
        }

        public static string GetInteractionText(this InteractionType interaction)
        {
            switch (interaction)
            {
                case InteractionType.Checkpoint:
                    return "Bind Soul";
                default:
                    return $"{interaction}";
            }
        }
    }

    public struct NullValue<T>
    {
        public T Value;
        public bool HasValue;

        public NullValue(T value)
        {
            Value = value;
            HasValue = true;
        }

        public void SetValue(T value)
        {
            Value = value;
            HasValue = true;
        }

    }
}