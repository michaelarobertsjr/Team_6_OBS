from flask import Flask, render_template, Blueprint, request
import jwt
import sqlalchemy as db
import pymysql

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
    elif request.method == "POST":
        username = request.form.get("username", None)
        password = request.form.get("password", None)
        email = request.form.get("email", None)

        #check whether all the data was passed in properly
        if username == None or password == None or email == None:
            return "Failed Request", 404

        #send to database
        sql = "INSERT INTO accounts (username, password, email) VALUES ('" + username + "','" + password + "','" + email + "')";
        num = app.config['DB_CONN'].execute(sql)

        return "Username already exists", 400

@app.route('/welcome')
def welcome():
    return render_template("obs_home.html")

if __name__ == "__main__" :

    app.run(debug=True)
