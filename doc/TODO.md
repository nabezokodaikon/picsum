# TODO
## リファクタリング 
* ローカル変数を`var`にする。
* プロパティをシンプルに定義する。
* 使われていない項目を削除する。
* 継承させないクラスに、`sealed`を記述する。
* 引数が3個以上の関数に、別途クラスを用意する。
### リファクタリング済みプロジェクト
* PicSum.Core.Base
* PicSum.Core.Data.DatabaseAccessor
* PicSum.Core.Task.AsyncTask
* PicSum.UIComponent.Common
## エラーハンドリング
* `DatabaseException`を作成する。
* 画像ファイルへのアクセスを、`ImageUtil`に集約する。
* ファイルへのアクセスを、`FileUtil`に集約する。
* `ImageUtil`の戻り値を、`Nullable`にする。
* `FileUtil`の戻り値を、`Nullable`や、件数`0`にする。
* `Exception`をキャッチせず、適切な例外のみをキャッチする。
* `throw new Exception`を適切な例外クラスにする。
## コマンドライン引数
* コマンドライン引数では実行しないようにする。
## ドロップダウン
* アドレスバーのドロップダウンが、0件のとき、ドロップダウンを表示しない。
## 起動時のページ
* ユーザーのホームディレクトリにする。
## アプリケーションの公開
* [Microsoft ストアへのアプリケーションの公開](https://sorceryforce.net/ja/tips/microsoft-store-release)
## エクスポート
* プログレスバーを実装する。
## ブックマーク
* `t_bookmark`テーブルを作成する。(トリガーも忘れずに)
* `t_bookmark`テーブルに対する、`INSERT`SQLを作成する。
* `t_bookmark`テーブルに対する、`DELETE`SQLを作成する。
* `t_bookmark`テーブルに追加するときは、先に`DELETE`SQLを実行する。
* `BookmarkFileListContents`コントロールを実装する。
* ブックマークボタンを追加する。
## コンテキストメニューのアイコン
* `Keep`アイコン
* `Bookmark`アイコン
* `Open in a new Tab`アイコン
    * Firefoxのアイコンを参考にする。
* `Open in a new Window`アイコン
    * Firefoxのアイコンを参考にする。
* `Copy Path`アイコン
    * `クリップボードアイコン`でググったら出てくる。
* `Copy Name`アイコン
    * `クリップボードアイコン`でググったら出てくる。
* `Export`アイコン
    * `エクスポートアイコン`でググったら出てくる。
* `関連付けて開く`アイコン
    * `ExtractAssociatedIcon関数`でできるらしい。
* `フォルダを開く`アイコン
    * `フォルダを開く アイコン`でググったら出てくる。
