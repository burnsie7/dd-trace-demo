-- groupname is not normalized and I don't care!
INSERT INTO user(name, groupname) VALUES ('Fred', 'apache')
INSERT INTO user(name, groupname) VALUES ('George', 'apache')
INSERT INTO user(name, groupname) VALUES ('Alice', 'pleb')
INSERT INTO user(name, groupname) VALUES ('Bob', 'pleb')
INSERT INTO user(name, groupname) VALUES ('Charles', 'pleb')

create alias MY_SLEEP for "java.lang.Thread.sleep";
