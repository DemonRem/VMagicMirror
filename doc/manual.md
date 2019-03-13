
# VMagicMirror マニュアル

目次

1. キャラクターを読み込んで動かす
2. 背景色と光の調整
3. キャラクターの動きとカメラ位置の調整
4. 設定のセーブとロード
5. トラブルシューティング

## 1. キャラクターを読み込んで動かす

`VMagicMirror.exe`を起動すると、小さな設定ウィンドウと大きなキャラクター表示ウィンドウが表示されます。

![Start Image](https://github.com/malaybaku/VMagicMirror/blob/master/doc/pic/started.png)

設定ウィンドウの「VRM読込」ボタンを押し、PC上の`.vrm`ファイルを選択します。

選択すると、キャラクター表示ウィンドウに規約などが表示されるため、確認して問題なければ`OK`をクリックします。

キャラクターが表示され、マウスやキーボードの操作に応じて動くようになります。


## 2. 背景色と光の調整

クロマキーを変更したい場合や、キャラクターの表示が明るすぎる/暗すぎる場合、設定ウィンドウ上部の「背景色と光」をクリックして設定を変更します。

`設定を保存`で設定をファイルに保存できます。
また、`設定をロード`で保存したファイルから設定をロードできます。

## 3. レイアウト

キャラクターの体型やカメラ位置になど、物理的な配置を編集する場合、設定ウィンドウの「レイアウト」をクリックして詳細ウィンドウを開きます。

* 手首の向きが明らかにおかしい場合、`手首から指先までの長さ[cm]`と`手首から手のひらまでの長さ[cm]`に、VRMの手のサイズに近い値を指定してください。
* `タッチタイピング風に視線を動かす`は、オン・オフを切り替えるとキャラクターの向く方向が変化します。
    * オン: マウスカーソルのある方向
    * オフ: 画面内で操作しているマウス、またはキーボードの方向


`設定を保存`で設定をファイルに保存できます。
また、`設定をロード`で保存したファイルから設定をロードできます。


## 4. スタートアップ設定

同じ設定を次回のアプリケーション起動時にも使いたい場合、設定ウィンドウの「スタートアップ」をクリックします。

それぞれのチェックボックスをオンにすることで、現在の表示設定が次回の起動時に自動で読み込まれます。

* いま表示しているキャラクター
* いまの背景色と光
* いまのカメラ位置、動き方の設定

## 5. トラブルシューティング

* マウスを動かしているのにキャラクターが動かなくなった場合、キャラクター表示ウィンドウをクリックしてみて下さい。
* 起動直後にアプリケーションが停止してしまう場合、`VMagicMirror.exe`があるフォルダに存在する`ConfigApp`フォルダを開いて以下のファイルを削除し、その後に`VMagicMirror`を再度起動してください。
    + `_currentBackground`
    + `_currentLayout`
    + `_currentVrm`
    + `_startup`