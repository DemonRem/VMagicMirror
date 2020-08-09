﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using VRM;

namespace Baku.VMagicMirror.ExternalTracker
{
    //TODO: 処理順が以下になってると多分正しいので、これがScript Execution Orderで保証されてるかチェック
    //> 1. (自動まばたき、LookAt、EyeJitter、パーフェクトシンクじゃないExTrackerまばたき)
    //> 2. このスクリプト
    //> 3. ExternalTrackerFaceSwitchApplier

    /// <summary> パーフェクトシンクをやるやつです。 </summary>
    /// <remarks>
    /// ブレンドシェイプクリップ名はhinzkaさんのセットアップに倣います。
    /// </remarks>
    public class ExternalTrackerPerfectSync : MonoBehaviour
    {
        /// <summary> 決め打ちされた、パーフェクトシンクで使うブレンドシェイプの一覧 </summary>
        static class Keys
        {
            static Keys()
            {
                PerfectSyncKeys = new[]
                {
                    //目
                    EyeBlinkLeft,
                    EyeLookUpLeft,
                    EyeLookDownLeft,
                    EyeLookInLeft,
                    EyeLookOutLeft,
                    EyeWideLeft,
                    EyeSquintLeft,

                    EyeBlinkRight,
                    EyeLookUpRight,
                    EyeLookDownRight,
                    EyeLookInRight,
                    EyeLookOutRight,
                    EyeWideRight,
                    EyeSquintRight,

                    //口(多い)
                    MouthLeft,
                    MouthSmileLeft,
                    MouthFrownLeft,
                    MouthPressLeft,
                    MouthUpperUpLeft,
                    MouthLowerDownLeft,
                    MouthStretchLeft,
                    MouthDimpleLeft,

                    MouthRight,
                    MouthSmileRight,
                    MouthFrownRight,
                    MouthPressRight,
                    MouthUpperUpRight,
                    MouthLowerDownRight,
                    MouthStretchRight,
                    MouthDimpleRight,

                    MouthClose,
                    MouthFunnel,
                    MouthPucker,
                    MouthShrugUpper,
                    MouthShrugLower,
                    MouthRollUpper,
                    MouthRollLower,

                    //あご
                    JawOpen,
                    JawForward,
                    JawLeft,
                    JawRight,

                    //鼻
                    NoseSneerLeft,
                    NoseSneerRight,

                    //ほお
                    CheekPuff,
                    CheekSquintLeft,
                    CheekSquintRight,

                    //舌
                    TongueOut,

                    //まゆげ
                    BrowDownLeft,
                    BrowOuterUpLeft,
                    BrowDownRight,
                    BrowOuterUpRight,
                    BrowInnerUp,
                };                
            }
        
            /// <summary> Perfect Syncでいじる対象のブレンドシェイプキー一覧を取得します。 </summary>
            public static BlendShapeKey[] PerfectSyncKeys { get; }
            
            //TODO: 名前はあとで調べて直すこと！絶対に間違った名前が入ってるぞ！
            
            //目
            public static readonly BlendShapeKey EyeBlinkLeft = new BlendShapeKey("eyeBlink_L");
            public static readonly BlendShapeKey EyeLookUpLeft = new BlendShapeKey("eyeLookUp_L");
            public static readonly BlendShapeKey EyeLookDownLeft = new BlendShapeKey("eyeLookDown_L");
            public static readonly BlendShapeKey EyeLookInLeft = new BlendShapeKey("eyeLookIn_L");
            public static readonly BlendShapeKey EyeLookOutLeft = new BlendShapeKey("eyeLookOut_L");
            public static readonly BlendShapeKey EyeWideLeft = new BlendShapeKey("eyeWide_L");
            public static readonly BlendShapeKey EyeSquintLeft = new BlendShapeKey("eyeSquint_L");

            public static readonly BlendShapeKey EyeBlinkRight = new BlendShapeKey("eyeBlink_R");
            public static readonly BlendShapeKey EyeLookUpRight = new BlendShapeKey("eyeLookUp_R");
            public static readonly BlendShapeKey EyeLookDownRight = new BlendShapeKey("eyeLookDown_R");
            public static readonly BlendShapeKey EyeLookInRight = new BlendShapeKey("eyeLookIn_R");
            public static readonly BlendShapeKey EyeLookOutRight = new BlendShapeKey("eyeLookOut_R");
            public static readonly BlendShapeKey EyeWideRight = new BlendShapeKey("eyeWide_R");
            public static readonly BlendShapeKey EyeSquintRight = new BlendShapeKey("eyeSquint_R");

            //口(多い)
            public static readonly BlendShapeKey MouthLeft = new BlendShapeKey(nameof(MouthLeft));
            public static readonly BlendShapeKey MouthSmileLeft = new BlendShapeKey("mouthSmile_L");
            public static readonly BlendShapeKey MouthFrownLeft = new BlendShapeKey("mouthFrown_L");
            public static readonly BlendShapeKey MouthPressLeft = new BlendShapeKey("mouthPress_L");
            public static readonly BlendShapeKey MouthUpperUpLeft = new BlendShapeKey("mouthUpperUp_L");
            public static readonly BlendShapeKey MouthLowerDownLeft = new BlendShapeKey("mouthLowerDown_L");
            public static readonly BlendShapeKey MouthStretchLeft = new BlendShapeKey("mouthStretch_L");
            public static readonly BlendShapeKey MouthDimpleLeft = new BlendShapeKey("mouthDimple_L");

            public static readonly BlendShapeKey MouthRight = new BlendShapeKey(nameof(MouthRight));
            public static readonly BlendShapeKey MouthSmileRight = new BlendShapeKey("mouthSmile_R");
            public static readonly BlendShapeKey MouthFrownRight = new BlendShapeKey("mouthFrown_R");
            public static readonly BlendShapeKey MouthPressRight = new BlendShapeKey("mouthPress_R");
            public static readonly BlendShapeKey MouthUpperUpRight = new BlendShapeKey("mouthUpperUp_R");
            public static readonly BlendShapeKey MouthLowerDownRight = new BlendShapeKey("mouthLowerDown_R");
            public static readonly BlendShapeKey MouthStretchRight = new BlendShapeKey("mouthStretch_R");
            public static readonly BlendShapeKey MouthDimpleRight = new BlendShapeKey("mouthDimple_R");
            
            public static readonly BlendShapeKey MouthClose = new BlendShapeKey(nameof(MouthClose));
            public static readonly BlendShapeKey MouthFunnel = new BlendShapeKey(nameof(MouthFunnel));
            public static readonly BlendShapeKey MouthPucker = new BlendShapeKey(nameof(MouthPucker));
            public static readonly BlendShapeKey MouthShrugUpper = new BlendShapeKey(nameof(MouthShrugUpper));
            public static readonly BlendShapeKey MouthShrugLower = new BlendShapeKey(nameof(MouthShrugLower));
            public static readonly BlendShapeKey MouthRollUpper = new BlendShapeKey(nameof(MouthRollUpper));
            public static readonly BlendShapeKey MouthRollLower = new BlendShapeKey(nameof(MouthRollLower));
            
            //あご
            public static readonly BlendShapeKey JawOpen = new BlendShapeKey(nameof(JawOpen));
            public static readonly BlendShapeKey JawForward = new BlendShapeKey(nameof(JawForward));
            public static readonly BlendShapeKey JawLeft = new BlendShapeKey(nameof(JawLeft));
            public static readonly BlendShapeKey JawRight = new BlendShapeKey(nameof(JawRight));
            
            //鼻
            public static readonly BlendShapeKey NoseSneerLeft = new BlendShapeKey("noseSneer_L");
            public static readonly BlendShapeKey NoseSneerRight = new BlendShapeKey("noseSneer_R");

            //ほお
            public static readonly BlendShapeKey CheekPuff = new BlendShapeKey(nameof(CheekPuff));
            public static readonly BlendShapeKey CheekSquintLeft = new BlendShapeKey("cheekSquint_L");
            public static readonly BlendShapeKey CheekSquintRight = new BlendShapeKey("cheekSquint_R");
            
            //舌
            public static readonly BlendShapeKey TongueOut = new BlendShapeKey(nameof(TongueOut));
            
            //まゆげ
            public static readonly BlendShapeKey BrowDownLeft = new BlendShapeKey("browDown_L");
            public static readonly BlendShapeKey BrowOuterUpLeft = new BlendShapeKey("browOuterUp_L");
            public static readonly BlendShapeKey BrowDownRight = new BlendShapeKey("browDown_Right");
            public static readonly BlendShapeKey BrowOuterUpRight = new BlendShapeKey("browOuterUp_R");
            public static readonly BlendShapeKey BrowInnerUp = new BlendShapeKey(nameof(BrowInnerUp));
        }

        [Tooltip("VRoidデフォルト設定が使いたいとき、元アバターのClipとこのAvatarのClipを融合させる")]
        [SerializeField] private BlendShapeAvatar vroidDefaultBlendShapeAvatar = null;

        private ExternalTrackerDataSource _externalTracker = null;
        private BlendShapeInitializer _blendShapeInitializer = null;
        private IMessageSender _sender = null;
        //VRoid向けのデフォルト設定に入ってるクリップのうち、パーフェクトシンクの分だけ抜き出したもの
        private List<BlendShapeClip> _vroidDefaultClips = null;
        
        private VRMBlendShapeProxy _blendShape = null;
        //このクラスがClipsを魔改造する前の時点でのクリップ一覧
        private List<BlendShapeClip> _modelBaseClips = null;
        private bool _hasModel = false;

        private bool _isActive = false;
        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    RefreshClips();
                }
            }
        }
        
        private bool _useVRoidSetting = false;
        public bool UseVRoidSetting
        {
            get => _useVRoidSetting;
            set
            {
                if (_useVRoidSetting != value)
                {
                    _useVRoidSetting = value;
                    RefreshClips();
                }
            } 
        }

        [Inject]
        public void Initialize(
            IMessageReceiver receiver, 
            IMessageSender sender,
            IVRMLoadable vrmLoadable,
            ExternalTrackerDataSource externalTracker,
            BlendShapeInitializer blendShapeInitializer
            )
        {
            _sender = sender;
            _externalTracker = externalTracker;
            _blendShapeInitializer = blendShapeInitializer;

            vrmLoadable.VrmLoaded += info =>
            {
                _blendShape = info.blendShape;
                //参照じゃなくて値コピーしとくことに注意(なにかと安全なので)
                _modelBaseClips = info.blendShape.BlendShapeAvatar.Clips.ToList();
                _hasModel = true;
            };

            vrmLoadable.PostVrmLoaded += info =>
            {
                //ロード直後にクリップ差し替えが必要ならやっておく
                if (UseVRoidSetting && IsActive)
                {
                    RefreshClips();
                }
            };

            vrmLoadable.VrmDisposing += () =>
            {
                _hasModel = false;
                _blendShape = null;
                _modelBaseClips = null;
            };
            
            receiver.AssignCommandHandler(
                VmmCommands.ExTrackerEnablePerfectSync,
                command => IsActive = command.ToBoolean()
            );
            receiver.AssignCommandHandler(
                VmmCommands.ExTrackerUseVRoidDefaultForPerfectSync,
                command => UseVRoidSetting = command.ToBoolean()
                );
        }

        private void LateUpdate()
        {
            if (!_hasModel || !IsActive)
            {
                return;
            }
            MapToBlendShapeClips();
        }
        
        /// <summary> 外部トラッキングで取得したブレンドシェイプをアバターに反映します。 </summary>
        private void MapToBlendShapeClips()
        {
            if (!_hasModel || _externalTracker.Connected)
            {
                return;
            }
            
            //とりあえず捨て
            _blendShapeInitializer.InitializeBlendShapes(false);
            _blendShape.Apply();
            
            //NOTE: とくにVRoidデフォルト設定を使わない場合、本来ほしいブレンドシェイプの一部が定義されてないと
            //「実際にはアバターが持ってないキーを指定してしまう」ということが起きるが、
            //これはBlendShapeMergerのレベルで実質無視してくれるので、気にせず指定しちゃってOK
            var source = _externalTracker.CurrentSource;

            //目
            var eye = source.Eye;
            _blendShape.AccumulateValue(Keys.EyeBlinkLeft, eye.LeftBlink);
            _blendShape.AccumulateValue(Keys.EyeLookUpLeft, eye.LeftLookUp);
            _blendShape.AccumulateValue(Keys.EyeLookDownLeft, eye.LeftLookDown);
            _blendShape.AccumulateValue(Keys.EyeLookInLeft, eye.LeftLookIn);
            _blendShape.AccumulateValue(Keys.EyeLookOutLeft, eye.LeftLookOut);
            _blendShape.AccumulateValue(Keys.EyeWideLeft, eye.LeftWide);
            _blendShape.AccumulateValue(Keys.EyeSquintLeft, eye.LeftSquint);

            _blendShape.AccumulateValue(Keys.EyeBlinkRight, eye.RightBlink);
            _blendShape.AccumulateValue(Keys.EyeLookUpRight, eye.RightLookUp);
            _blendShape.AccumulateValue(Keys.EyeLookDownRight, eye.RightLookDown);
            _blendShape.AccumulateValue(Keys.EyeLookInRight, eye.RightLookIn);
            _blendShape.AccumulateValue(Keys.EyeLookOutRight, eye.RightLookOut);
            _blendShape.AccumulateValue(Keys.EyeWideRight, eye.RightWide);
            _blendShape.AccumulateValue(Keys.EyeSquintRight, eye.RightSquint);
            
            //口(多い)
            var mouth = source.Mouth;
            _blendShape.AccumulateValue(Keys.MouthLeft, mouth.Left);
            _blendShape.AccumulateValue(Keys.MouthSmileLeft, mouth.LeftSmile);
            _blendShape.AccumulateValue(Keys.MouthFrownLeft, mouth.LeftFrown);
            _blendShape.AccumulateValue(Keys.MouthPressLeft, mouth.LeftPress);
            _blendShape.AccumulateValue(Keys.MouthUpperUpLeft, mouth.LeftUpperUp);
            _blendShape.AccumulateValue(Keys.MouthLowerDownLeft, mouth.LeftLowerDown);
            _blendShape.AccumulateValue(Keys.MouthStretchLeft, mouth.LeftStretch);
            _blendShape.AccumulateValue(Keys.MouthDimpleLeft, mouth.LeftDimple);

            _blendShape.AccumulateValue(Keys.MouthRight, mouth.Right);
            _blendShape.AccumulateValue(Keys.MouthSmileRight, mouth.RightSmile);
            _blendShape.AccumulateValue(Keys.MouthFrownRight, mouth.RightFrown);
            _blendShape.AccumulateValue(Keys.MouthPressRight, mouth.RightPress);
            _blendShape.AccumulateValue(Keys.MouthUpperUpRight, mouth.RightUpperUp);
            _blendShape.AccumulateValue(Keys.MouthLowerDownRight, mouth.RightLowerDown);
            _blendShape.AccumulateValue(Keys.MouthStretchRight, mouth.RightStretch);
            _blendShape.AccumulateValue(Keys.MouthDimpleRight, mouth.RightDimple);

            _blendShape.AccumulateValue(Keys.MouthClose, mouth.Close);
            _blendShape.AccumulateValue(Keys.MouthFunnel, mouth.Funnel);
            _blendShape.AccumulateValue(Keys.MouthPucker, mouth.Pucker);
            _blendShape.AccumulateValue(Keys.MouthShrugUpper, mouth.ShrugUpper);
            _blendShape.AccumulateValue(Keys.MouthShrugLower, mouth.ShrugLower);
            _blendShape.AccumulateValue(Keys.MouthRollUpper, mouth.RollUpper);
            _blendShape.AccumulateValue(Keys.MouthRollLower, mouth.RollLower);

            //あご
            _blendShape.AccumulateValue(Keys.JawOpen, source.Jaw.Open);
            _blendShape.AccumulateValue(Keys.JawForward, source.Jaw.Forward);
            _blendShape.AccumulateValue(Keys.JawLeft, source.Jaw.Left);
            _blendShape.AccumulateValue(Keys.JawRight, source.Jaw.Right);

            //鼻
            _blendShape.AccumulateValue(Keys.NoseSneerLeft, source.Nose.LeftSneer);
            _blendShape.AccumulateValue(Keys.NoseSneerRight, source.Nose.RightSneer);

            //ほお
            _blendShape.AccumulateValue(Keys.CheekPuff, source.Cheek.Puff);
            _blendShape.AccumulateValue(Keys.CheekSquintLeft, source.Cheek.LeftSquint);
            _blendShape.AccumulateValue(Keys.CheekSquintRight, source.Cheek.RightSquint);

            //舌
            _blendShape.AccumulateValue(Keys.TongueOut, source.Tongue.TongueOut);

            //まゆげ
            _blendShape.AccumulateValue(Keys.BrowDownLeft, source.Brow.LeftDown);
            _blendShape.AccumulateValue(Keys.BrowOuterUpLeft, source.Brow.LeftOuterUp);
            _blendShape.AccumulateValue(Keys.BrowDownRight, source.Brow.RightDown);
            _blendShape.AccumulateValue(Keys.BrowOuterUpRight, source.Brow.RightOuterUp);
            _blendShape.AccumulateValue(Keys.BrowInnerUp, source.Brow.InnerUp);
        }

        
        private void RefreshClips()
        {
            if (!_hasModel)
            {
                return;
            }
            
            //差し替え前後で表情が崩れないよう完全にリセット
            _blendShape.Apply();
            _blendShapeInitializer.InitializeBlendShapes(false);
            _blendShape.Apply();

            //パーフェクトシンクが不要とか、モデル本体に設定がある場合、本来のクリップが入ってればOK
            if (!IsActive || !UseVRoidSetting)
            {
                _blendShape.BlendShapeAvatar.Clips = _modelBaseClips.ToList();
                //TODO: このリロードがUniVRM書き換えになるのがヤなので、別の方法があれば検討したい…
                _blendShape.ReloadBlendShape();
                return;
            }

            //オリジナルをベースにしつつ、パーフェクトシンク用のクリップだけはVRoidデフォルトので上書きしていく
            var overwriteClips = Keys.PerfectSyncKeys;
            _blendShape.BlendShapeAvatar.Clips = _modelBaseClips
                .Where(c =>
                {
                    var key = new BlendShapeKey(c.BlendShapeName, c.Preset);
                    return !overwriteClips.Contains(key);
                })
                .Concat(_vroidDefaultClips)
                .ToList();
            _blendShape.ReloadBlendShape();
        }
    }
}
