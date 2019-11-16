from flask import Flask, render_template, Blueprint, request
import jwt
import sqlalchemy as db
import pymysql
import time

app = Flask(__name__)

app.config['SECRET'] = "XCAP05H6LoKvbRRa/QkqLNMI7cOHguaRyHzyg7n5qEkGjQmtBhz4SzYh4Fqwjyi3KJHlSXKPwVu2+bXr6CtpgQ=="
app.config['DB_HOST'] = ""
app.config['DB_USER'] = ""
app.config['DB_PASS'] = ""
app.config['DB_NAME'] = ""

engine = db.create_engine('mysql+pymysql://' + app.config['DB_USER'] + ':' + app.config['DB_PASS'] + '@' + app.config['DB_HOST'] + '/' + app.config['DB_NAME'], pool_pre_ping=True)
app.config['DB_CONN'] = engine.connect()

@app.route('/')
def home():
    return render_template("obs_navigation.html")

@app.route('/signup', methods=["GET", "POST"])
def signup():
    if request.method == "GET":
        return render_template("signup.html")
    if request.method == "POST":
        username = request.form.get("username", None)
        password = request.form.get("password", None)
        email = request.form.get("email", None)

        #check whether all the data was passed in properly
        if username == None or password == None or email == None:
            return "Failed Request", 404

        #check database for existing user
        sql = 'SELECT uid, username, email FROM accounts WHERE email=\'' + email + '\''
        existing = app.config['DB_CONN'].execute(sql).fetchall()
        if len(existing) == 0:
            #send to database
            sql = 'INSERT INTO accounts (username, password, email) VALUES (\'' + username + '\',\'' + password + '\',\'' + email + '\');'
            num = app.config['DB_CONN'].execute(sql)

            #query for additional auto-generated user info
            sql = 'SELECT uid, username, email FROM accounts WHERE email=\'' + email + '\''
            test = app.config['DB_CONN'].execute(sql).fetchall()

            epoch_time = int(time.time()) + 3600   #gets the epoch time in UTC this is used as an expiration for JWT and add an hour
            payload = {'username' : test[0][1], 'email' : test[0][2], 'exp' : epoch_time}
            token = jwt.encode(payload, app.config['SECRET'], algorithm='HS256')
            return token, 200
        else:
            return "Email address already in use", 400

        return "Success!", 200

@app.route('/login', methods=["GET", "POST"])
def login():
    if request.method == "GET":
        return "Success", 404
    if request.method == "POST":
        username = request.form.get("username")
        password = request.form.get("password")

        #check whether all the data was passed in properly
        if username == None or password == None:
            return "Failed Request", 404

        sql = 'SELECT * FROM accounts WHERE username=\'' + username + '\' AND password=\'' + password + '\''
        test = app.config['DB_CONN'].execute(sql).fetchall()
        #Add form input cases

        if len(test) != 0:
            epoch_time = int(time.time()) + 3600   #gets the epoch time in UTC this is used as an expiration for JWT and add an hour
            payload = {'username' : test[0][1], 'email' : test[0][3], 'exp': epoch_time}
            token = jwt.encode(payload, app.config['SECRET'], algorithm='HS256')
            print(token)
            return token, 200
        else:
            return "Invalid User Credentials", 400

@app.route('/welcome')
def welcome():
    return render_template("obs_home.html")

if __name__ == "__main__" :

    app.run(debug=True)
