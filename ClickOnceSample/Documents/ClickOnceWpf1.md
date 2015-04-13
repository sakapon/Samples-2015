### Web アプリケーションの設定
* Web.config で既定のドキュメントに ClickOnceWpf1.application を追加する

### 証明書
* abc-root.pfx を証明書ストアの個人 (My) にインストールする

### プロジェクトの設定

#### 署名
[ストアから選択] → [Abc Root]

#### 発行
* 発行フォルダーの場所: ..\ClickOnceWeb1\
* インストール フォルダーの URL: http://localhost:45865/
* 発行者名: Abc Publisher
* スイート名: Abc WPF
* 製品名: ClickOnce WPF 1

### 発行
[今すぐ発行]

### 実行結果
* 証明書を登録しない: 赤色の警告
* 証明書を Root に登録: 黄色の警告 (ローカル ファイルの場合は緑)
* 証明書を Root, TrustedPublisher に登録: 起動
