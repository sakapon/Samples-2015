### Web アプリケーションの設定
* Web.config で既定のドキュメントに ClickOnceWpf1.application を追加する

### 証明書
* abc-root.pfx を証明書ストアの個人 (My) にインストールする

### プロジェクトの設定

#### 署名
[ストアから選択] → [Abc Root]

#### 発行
* 発行フォルダーの場所: ..\ClickOnceWeb1\
* 発行者名: Abc Publisher
* スイート名: Abc WPF
* 製品名: ClickOnce WPF 1

### 発行
* ソリューション構成を [Release] に切り替える
* [今すぐ発行]

### 実行結果
* 証明書を登録しない: 赤色の警告
* 証明書を Root に登録: 黄色の警告 (ローカル ファイルの場合は緑)
* 証明書を Root, TrustedPublisher に登録: 起動

### 自動更新
アプリケーションの自動更新を有効にする場合、[インストール フォルダーの URL] を指定する。
ただし、localhost の Web サーバーでは失敗する。
