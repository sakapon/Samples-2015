### Web アプリケーションの設定
* Web.config で既定のドキュメントに ClickOnceWpf1.application を追加する

### 証明書
* abc-root.pfx を証明書ストアの My にインストールする

### プロジェクトの設定

#### 署名
[ストアから選択] → [Abc Root]

#### 発行
* 発行フォルダーの場所: ..\ClickOnceWeb1\
* インストール フォルダーの URL: https://sakapon.github.io/publish-test/ClickOnceWpf1/ (Web に配置する場合は指定する)
* 発行者名: Abc Publisher
* スイート名: Abc WPF
* 製品名: ClickOnce WPF 1

### 発行
* ソリューション構成を [Release] に切り替える
* [今すぐ発行]

### 実行結果
クライアント PC に登録する証明書は .cer。

#### ローカルに配置した場合
* 証明書を登録しない: [公開元] は赤色
* 証明書を TrustedPublisher に登録: [公開元] は赤色
* 証明書を Root に登録: [公開元] は緑色
* 証明書を Root, TrustedPublisher に登録: 起動

#### Web に配置した場合
基本的にはローカルに配置した場合と同様だが、インストール後の初回起動時に Windows SmartScreen の警告が表示される。

### 自動更新
アプリケーションの自動更新を有効にする場合、[インストール フォルダーの URL] を指定する。  
ただし、localhost の Web サーバーでは失敗する。

### 用語
* My: 個人
* Root: 信頼されたルート証明機関
* TrustedPublisher: 信頼された発行元
